using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAccess.Repositories;
using DinkToPdf;
using DinkToPdf.Contracts;
using AutoServicesWebSite.Code;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace AutoServicesWebSite.Controllers
{
    [Authorize]
    public class ProtocolController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(ProtocolController));
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IConverter _converter;
        public ProtocolController(IRepositoryWrapper repoWrapper, IConverter converter)
        {
            _repoWrapper = repoWrapper;
            _converter = converter;
        }
        // GET: Protocol
        [HttpGet]
        public ActionResult History()
        {
            var sevenDaysAgo = DateTime.Now;
            DateTime dateFrom = new DateTime(sevenDaysAgo.Year, sevenDaysAgo.Month, sevenDaysAgo.Day, 0, 0, 0);
            DateTime dateTo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            ViewBag.dateFromString = dateFrom.ToString("dd-MM-yyyy");
            ViewBag.dateToString = dateTo.ToString("dd-MM-yyyy");
            ViewBag.dateFrom = dateFrom.ToString("s");
            ViewBag.dateTo = dateTo.ToString("s");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult History(DateTime dateFrom, DateTime dateTo, int SlujNomer, string RegNomer, string Mehanik)
        {
            dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
            dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);
            IList<Protocol> model = _repoWrapper.Protocol.Query()
               .Where(
                p => (p.Data > dateFrom.AddDays(-1) && p.Data < dateTo.AddDays(1) && // filter by Date
                (SlujNomer == 0) || p.SlujNomer == SlujNomer) &&
                (string.IsNullOrEmpty(RegNomer) || p.RegNomer == RegNomer) && // filter by RegNomer
                (string.IsNullOrEmpty(Mehanik) || p.Mehanik == Mehanik)) // filter by Mehanik
               .OrderByDescending(p => p.ProtocolId).ToList();
            ViewBag.dateFromString = dateFrom.ToString("dd-MM-yyyy");
            ViewBag.dateToString = dateTo.ToString("dd-MM-yyyy");
            ViewBag.dateFrom = dateFrom.ToString("s"); ;
            ViewBag.dateTo = dateTo.ToString("s");
            ViewBag.SlujNomer = SlujNomer;
            ViewBag.RegNomer = RegNomer;
            ViewBag.Mehanik = Mehanik;
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult HistoryFromAjax(DateTime dateFrom, DateTime dateTo, int SlujNomer, string RegNomer, string Mehanik)
        {
            dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
            dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 59);

            var model =
             _repoWrapper.Protocol.Query()
               .Where(
                p => (p.Data > dateFrom && p.Data < dateTo && // filter by Date
                (SlujNomer == 0) || p.SlujNomer == SlujNomer) &&
                (string.IsNullOrEmpty(RegNomer) || p.RegNomer == RegNomer) && // filter by RegNomer
                (string.IsNullOrEmpty(Mehanik) || p.Mehanik == Mehanik)) // filter by Mehanik
               .OrderByDescending(p => p.ProtocolId).ToList();


            return ViewComponent("History", model);
        }
        public ActionResult Index()
        {
            return View();
        }

        // GET: Protocol/Details/5
        public ActionResult Details(int protocolId)
        {
            Protocol model = _repoWrapper.Protocol.GetById(protocolId);
            return PartialView(model);
        }

        // GET: Protocol/Create
        [HttpGet]
        public ActionResult Add(int id = 0)
        {
            var loggedUserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var placeId = HttpContext.Session.GetInt32("PlaceId") ?? (_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).Any() ? (int?)_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).FirstOrDefault().PlaceId : null);
            if (!HttpContext.Session.GetInt32("PlaceId").HasValue && placeId.HasValue)
            {
                HttpContext.Session.SetInt32("PlaceId", placeId.Value);
            }
            if (placeId == null)
            {
                HttpContext.SignOutAsync();
                return RedirectToAction("Login", "User");
            }
            else if (placeId.HasValue && _repoWrapper.Place.IsBlocked(placeId.Value))
            {
                try
                {
                    var place = _repoWrapper.Place.GetById((int)placeId);
                    place.UserId = null;
                    place.IsBusy = false;
                    _repoWrapper.Place.Update(place);
                    _repoWrapper.Place.Save();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                HttpContext.SignOutAsync();
                return RedirectToAction("Login", "User");
            }
            var queue = id > 0 ? _repoWrapper.Queue.GetIncomplete(placeId.Value) : _repoWrapper.Queue.GetNextInLine();

            if (queue == null)
                return RedirectToAction("Index", "Home");

            int waitingTime = _repoWrapper.Setting.Setting.WaitingTime;
            ViewBag.WaitingTime = waitingTime;

            int specificNumber;
            string optionName = string.Empty;

            var option = _repoWrapper.OptionQueue.Query().Where(oq => oq.QueueId == queue.QueueId).ToList();
            for (int i = 0; i < option.Count; i++)
            {
                optionName += _repoWrapper.Option.GetById(option[i].OptionId).Name;
                if (i < option.Count - 1)
                    optionName += " + ";
            }
            var isSlujNumber = Int32.TryParse(queue.SpecificNumber, out specificNumber);
            var user = User.FindFirstValue(ClaimTypes.Name);
            ProtocolView model = new ProtocolView
            {
                SlujNomer = isSlujNumber ? (int?)specificNumber : null,
                RegNomer = !isSlujNumber ? queue.SpecificNumber : null,
                Data = DateTime.Now,
                QueueId = queue.QueueId,
                OptionName = optionName,
                Mehanik = user,
                InLine = id > 0

            };
            //update place for a client in the queue
            if (placeId != null)
            {
                queue.PlaceId = placeId;
                try
                {
                    _repoWrapper.Queue.Update(queue);
                    _repoWrapper.Queue.Save();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
            }
            return View(model);
        }

        // POST: Protocol/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProtocolView model)
        {
            var loggedUserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var placeId = HttpContext.Session.GetInt32("PlaceId") ?? (_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).Any() ? (int?)_repoWrapper.Place.Query().Where(p => !p.IsBlock && p.UserId == loggedUserId).FirstOrDefault().PlaceId : null);
            if (!HttpContext.Session.GetInt32("PlaceId").HasValue && placeId.HasValue)
            {
                HttpContext.Session.SetInt32("PlaceId", placeId.Value);
            }
            if (placeId.HasValue && _repoWrapper.Place.IsBlocked(placeId.Value))
            {
                try
                {
                    var place = _repoWrapper.Place.GetById((int)placeId);
                    place.UserId = null;
                    _repoWrapper.Place.Update(place);
                    _repoWrapper.Place.Save();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message + ex.InnerException.Message);
                }
                HttpContext.SignOutAsync();
                return RedirectToAction("Login", "User");
            }
            int waitingTime = _repoWrapper.Setting.Setting.WaitingTime;
            ViewBag.WaitingTime = waitingTime;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //else if (!model.AvtomobilatENeIzpraven && !model.AvtomobilatEIzpraven)
            //{
            //    ModelState.AddModelError("Грешка", "Не сте отбелязали дали е изправен или не.");
            //    return View(model);
            //}
            Queue queue = null;
            try
            {
                //add protocol
                model.Data = DateTime.Now;
                model.IsExternal = false;
                _repoWrapper.Protocol.Add(model);
                _repoWrapper.Protocol.Save();

                //update queue
                queue = _repoWrapper.Queue.GetById(model.QueueId);
                queue.IsActive = false;
                _repoWrapper.Queue.Update(queue);
                _repoWrapper.Queue.Save();

                //update place
                if (model.IsBlock)
                {
                    if (placeId != null)
                    {
                        var place = _repoWrapper.Place.GetById((int)placeId);
                        place.IsBlock = true;
                        place.ProtocolId = model.ProtocolId;

                        _repoWrapper.Place.Update(place);
                        _repoWrapper.Place.Save();
                    }
                }


                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                log.Error("Method: Add, " + ex.ToString() + "\nInnerException:" + ex.InnerException.ToString());
                log.Error("\nProtocol Data:" + JsonConvert.SerializeObject(model));
                log.Error("\nQueue Data:" + JsonConvert.SerializeObject(queue));

            }
            return View(model);
        }

        // POST: Protocol/Create


        [HttpGet]
        public ActionResult AddExternal()
        {
            var places = _repoWrapper.Place.GetActive();
            var user = User.FindFirstValue(ClaimTypes.Name);
            ProtocolView model = new ProtocolView
            {
                Data = DateTime.Now,
                Place = places,
                Mehanik = user
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExternal(ProtocolView model)
        {
            var places = _repoWrapper.Place.GetActive();
            model.Place = places;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else if (model.SlujNomer == null && model.RegNomer == null)
            {
                ModelState.AddModelError("Грешка", "Попълнете Служебен № или Рег №.");
                return View(model);
            }
            //else if (!model.AvtomobilatENeIzpraven && !model.AvtomobilatEIzpraven)
            //{
            //    ModelState.AddModelError("Грешка", "Не сте отбелязали дали е изправен или не.");
            //    return View(model);
            //}
            try
            {
                //add protocol
                model.Data = DateTime.Now;
                model.IsExternal = true;
                _repoWrapper.Protocol.Add(model);
                _repoWrapper.Protocol.Save();

                //update place
                if (model.IsBlock)
                {
                    if (model.PlaceId > 0)
                    {
                        var place = _repoWrapper.Place.GetById(model.PlaceId);
                        place.IsBlock = true;
                        place.ProtocolId = model.ProtocolId;

                        _repoWrapper.Place.Update(place);
                        _repoWrapper.Place.Save();
                    }
                }


                return RedirectToAction("History", "Protocol");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
            return View(model);
        }

        [HttpGet]
        [Route("[controller]/[action]/{protocolId}")]
        public ActionResult Edit(int protocolId)
        {
            var model = _repoWrapper.Protocol.GetById(protocolId);
            return View(model);
        }

        // POST: Protocol/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Protocol model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else if (model.SlujNomer == null && model.RegNomer == null)
            {
                ModelState.AddModelError("Грешка", "Попълнете Служебен № или Рег №.");
                return View(model);
            }
            //else if (!model.AvtomobilatENeIzpraven && !model.AvtomobilatEIzpraven)
            //{
            //    ModelState.AddModelError("Грешка", "Не сте отбелязали дали е изправен или не.");
            //    return View(model);
            //}
            try
            {
                if (model.IsBlock)
                {
                    var place = _repoWrapper.Place.Query().Where(p => p.ProtocolId == model.ProtocolId && p.IsBlock).FirstOrDefault();
                    place.IsBlock = false;
                    place.ProtocolId = null;

                    _repoWrapper.Place.Update(place);
                    _repoWrapper.Place.Save();
                }
                //add protocol
                model.IsBlock = false;
                _repoWrapper.Protocol.Update(model);
                _repoWrapper.Protocol.Save();

                //update place



                return RedirectToAction("History", "Protocol");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);

            }
            return View(model);
        }

        // GET: Protocol/Delete/5


        // POST: Protocol/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(ProtocolView model)
        {
            try
            {
                //update queue
                var queue = _repoWrapper.Queue.GetById(model.QueueId);
                queue.IsActive = false;
                queue.PlaceId = null;
                queue.SpecificNumber = null;

                _repoWrapper.Queue.Update(queue);
                _repoWrapper.Queue.Save();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelExternal()
        {
            return RedirectToAction("History", "Protocol");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("[controller]/Transfered")]

        public void Transfered(int protocolId, bool isTransfered)
        {
            try
            {
                Protocol model = _repoWrapper.Protocol.GetById(protocolId);
                model.IsTransfered = isTransfered;
                _repoWrapper.Protocol.Update(model);
                _repoWrapper.Protocol.Save();
            }
            catch (Exception ex)
            {
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task ExportProtocol(int ProtocolId)
        {
            Protocol protocol = _repoWrapper.Protocol.GetById(ProtocolId);
            if (protocol == null)
                return;
            string fileName = @"{0}-{1}.xlsx";
            fileName = string.Format(fileName, protocol.Data.ToString("yyyy-MM-dd"), protocol.ProtocolId);

            IWorkbook workbook = new XSSFWorkbook();
            ISheet excelSheet = workbook.CreateSheet();

            IRow rowHeader = excelSheet.CreateRow(0);
            IRow rowData = excelSheet.CreateRow(1);
            rowHeader.CreateCell(0).SetCellValue("Протокол №");
            rowData.CreateCell(0).SetCellValue(protocol.ProtocolId.ToString());
            rowHeader.CreateCell(1).SetCellValue("Дата");
            rowData.CreateCell(1).SetCellValue(protocol.Data.ToString("dd/MM/yyyy"));
            rowHeader.CreateCell(2).SetCellValue("Служебен №");
            rowData.CreateCell(2).SetCellValue(protocol.SlujNomer.ToString());
            rowHeader.CreateCell(3).SetCellValue("Рег. №");
            rowData.CreateCell(3).SetCellValue(protocol.RegNomer);
            rowHeader.CreateCell(4).SetCellValue("Пробег");
            rowData.CreateCell(4).SetCellValue(protocol.Probeg.ToString());
            rowHeader.CreateCell(5).SetCellValue("Водач");
            rowData.CreateCell(5).SetCellValue(protocol.Vodach);

            rowHeader.CreateCell(6).SetCellValue("ВЪНШЕН ВИД ХИГИЕНА");
            rowData.CreateCell(6).SetCellValue(protocol.VVHigiena);
            rowHeader.CreateCell(7).SetCellValue("ВЪНШЕН ВИД ХИГИЕНА");
            rowData.CreateCell(7).SetCellValue(protocol.VVHigienaDaNe);
            rowHeader.CreateCell(8).SetCellValue("ВЪНШЕН ВИД БРОНИ");
            rowData.CreateCell(8).SetCellValue(protocol.VVBroni);
            rowHeader.CreateCell(9).SetCellValue("ВЪНШЕН ВИД БРОНИ");
            rowData.CreateCell(9).SetCellValue(protocol.VVBroniDaNe);
            rowHeader.CreateCell(10).SetCellValue("ВЪНШЕН ВИД КАЛНИЦИ");
            rowData.CreateCell(10).SetCellValue(protocol.VVKalnici);
            rowHeader.CreateCell(11).SetCellValue("ВЪНШЕН ВИД КАЛНИЦИ");
            rowData.CreateCell(11).SetCellValue(protocol.VVKalniciDaNe);
            rowHeader.CreateCell(12).SetCellValue("ВЪНШЕН ВИД ВРАТИ");
            rowData.CreateCell(12).SetCellValue(protocol.VVVrati);
            rowHeader.CreateCell(13).SetCellValue("ВЪНШЕН ВИД ВРАТИ");
            rowData.CreateCell(13).SetCellValue(protocol.VVVratiDaNe);
            rowHeader.CreateCell(14).SetCellValue("ВЪНШЕН ВИД СТЪКЛА");
            rowData.CreateCell(14).SetCellValue(protocol.VVStykla);
            rowHeader.CreateCell(15).SetCellValue("ВЪНШЕН ВИД СТЪКЛА");
            rowData.CreateCell(15).SetCellValue(protocol.VVStyklaDaNe);
            rowHeader.CreateCell(16).SetCellValue("ВЪНШЕН ВИД ДРУГИ");
            rowData.CreateCell(16).SetCellValue(protocol.VVDrugi);

            rowHeader.CreateCell(17).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ДВИГАТЕЛ");
            rowData.CreateCell(17).SetCellValue(protocol.ZADvigatel);
            rowHeader.CreateCell(18).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ДВИГАТЕЛ");
            rowData.CreateCell(18).SetCellValue(protocol.ZADvigatelDaNe);
            rowHeader.CreateCell(19).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ РЕМЪЦИ");
            rowData.CreateCell(19).SetCellValue(protocol.ZaRemyci);
            rowHeader.CreateCell(20).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ РЕМЪЦИ");
            rowData.CreateCell(20).SetCellValue(protocol.ZaRemyciDaNe);
            rowHeader.CreateCell(21).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ОХЛАЖДАЩА УРЕДБА");
            rowData.CreateCell(21).SetCellValue(protocol.ZAOhlajdashtaUredba);
            rowHeader.CreateCell(22).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ОХЛАЖДАЩА УРЕДБА");
            rowData.CreateCell(22).SetCellValue(protocol.ZAOhlajdashtaUredbaDaNe);
            rowHeader.CreateCell(23).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ГОРИВНА УРЕДБА");
            rowData.CreateCell(23).SetCellValue(protocol.ZAGorivnaUredba);
            rowHeader.CreateCell(24).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ГОРИВНА УРЕДБА");
            rowData.CreateCell(24).SetCellValue(protocol.ZAGorivnaUredbaDaNe);
            rowHeader.CreateCell(25).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ИЗПУСКАТЕЛНА УРЕДБА");
            rowData.CreateCell(25).SetCellValue(protocol.ZAIzpuskatelnaUredba);
            rowHeader.CreateCell(26).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ИЗПУСКАТЕЛНА УРЕДБА");
            rowData.CreateCell(26).SetCellValue(protocol.ZAIzpuskatelnaUredbaDaNe);
            rowHeader.CreateCell(27).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ МАЗИЛНА УРЕДБА");
            rowData.CreateCell(27).SetCellValue(protocol.ZAMazilnaUredba);
            rowHeader.CreateCell(28).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ МАЗИЛНА УРЕДБА");
            rowData.CreateCell(28).SetCellValue(protocol.ZAMazilnaUredbaDaNe);
            rowHeader.CreateCell(29).SetCellValue("ЗАДВИЖВАЩ АГРЕГАТ ДРУГИ");
            rowData.CreateCell(29).SetCellValue(protocol.ZADrugi);

            rowHeader.CreateCell(30).SetCellValue("ТРАНСМИСИЯ СЪЕДИНИТЕЛ");
            rowData.CreateCell(30).SetCellValue(protocol.TMSyedinitel);
            rowHeader.CreateCell(31).SetCellValue("ТРАНСМИСИЯ СЪЕДИНИТЕЛ");
            rowData.CreateCell(31).SetCellValue(protocol.TMSyedinitelDaNe);
            rowHeader.CreateCell(32).SetCellValue("ТРАНСМИСИЯ СКОРОСТНА КУТИЯ И ДИФЕРЕНЦИАЛ");
            rowData.CreateCell(32).SetCellValue(protocol.TMSkorostnaKutiqIDiferencial);
            rowHeader.CreateCell(33).SetCellValue("ТРАНСМИСИЯ СКОРОСТНА КУТИЯ И ДИФЕРЕНЦИАЛ");
            rowData.CreateCell(33).SetCellValue(protocol.TMSkorostnaKutiqIDiferencialDaNe);
            rowHeader.CreateCell(34).SetCellValue("ТРАНСМИСИЯ ДРУГИ");
            rowData.CreateCell(34).SetCellValue(protocol.TMDrugi);

            rowHeader.CreateCell(35).SetCellValue("ХОДОВА ЧАСТ КОЛЕЛА");
            rowData.CreateCell(35).SetCellValue(protocol.HCHodoviKolela);
            rowHeader.CreateCell(36).SetCellValue("ХОДОВА ЧАСТ КОЛЕЛА");
            rowData.CreateCell(36).SetCellValue(protocol.HCHodoviKolelaDaNe);
            rowHeader.CreateCell(37).SetCellValue("ХОДОВА ЧАСТ ТАМПОНИ МФ");
            rowData.CreateCell(37).SetCellValue(protocol.HCTamponiMF);
            rowHeader.CreateCell(38).SetCellValue("ХОДОВА ЧАСТ ТАМПОНИ МФ");
            rowData.CreateCell(38).SetCellValue(protocol.HCTamponiMFDaNe);
            rowHeader.CreateCell(39).SetCellValue("ХОДОВА ЧАСТ АМОРТИЬОРИ ПРЕДНИ");
            rowData.CreateCell(39).SetCellValue(protocol.HCAmortisioriPredni);
            rowHeader.CreateCell(40).SetCellValue("ХОДОВА ЧАСТ АМОРТИЬОРИ ПРЕДНИ");
            rowData.CreateCell(40).SetCellValue(protocol.HCAmortisioriPredniDaNe);
            rowHeader.CreateCell(41).SetCellValue("ХОДОВА ЧАСТ НОСАЧИ ПРЕДНИ");
            rowData.CreateCell(41).SetCellValue(protocol.HCNosachiPredni);
            rowHeader.CreateCell(42).SetCellValue("ХОДОВА ЧАСТ НОСАЧИ ПРЕДНИ");
            rowData.CreateCell(42).SetCellValue(protocol.HCNosachiPredniDaNe);
            rowHeader.CreateCell(43).SetCellValue("ХОДОВА ЧАСТ ШАРНИРНИ БОЛТОВЕ");
            rowData.CreateCell(43).SetCellValue(protocol.HCSharnirniBoltove);
            rowHeader.CreateCell(44).SetCellValue("ХОДОВА ЧАСТ ШАРНИРНИ БОЛТОВЕ");
            rowData.CreateCell(44).SetCellValue(protocol.HCSharnirniBoltoveDaNe);
            rowHeader.CreateCell(45).SetCellValue("ХОДОВА ЧАСТ КАРЕТА");
            rowData.CreateCell(45).SetCellValue(protocol.HCKareta);
            rowHeader.CreateCell(46).SetCellValue("ХОДОВА ЧАСТ КАРЕТА");
            rowData.CreateCell(46).SetCellValue(protocol.HCKaretaDaNe);
            rowHeader.CreateCell(47).SetCellValue("ХОДОВА ЧАСТ МАНШОНИ");
            rowData.CreateCell(47).SetCellValue(protocol.HCManshoni);
            rowHeader.CreateCell(48).SetCellValue("ХОДОВА ЧАСТ МАНШОНИ");
            rowData.CreateCell(48).SetCellValue(protocol.HCManshoniDaNe);
            rowHeader.CreateCell(49).SetCellValue("ХОДОВА ЧАСТ НОСАЧИ ЗАДНИ");
            rowData.CreateCell(49).SetCellValue(protocol.HCNosachiZadni);
            rowHeader.CreateCell(50).SetCellValue("ХОДОВА ЧАСТ НОСАЧИ ЗАДНИ");
            rowData.CreateCell(50).SetCellValue(protocol.HCNosachiZadniDaNe);
            rowHeader.CreateCell(51).SetCellValue("ХОДОВА ЧАСТ АМОРТИСЬОРИ ЗАДНИ");
            rowData.CreateCell(51).SetCellValue(protocol.HCAmortisioriZadni);
            rowHeader.CreateCell(52).SetCellValue("ХОДОВА ЧАСТ АМОРТИСЬОРИ ЗАДНИ");
            rowData.CreateCell(52).SetCellValue(protocol.HCAmortisioriZadniDaNe);
            rowHeader.CreateCell(53).SetCellValue("ХОДОВА ЧАСТ ЛАГЕРИ");
            rowHeader.CreateCell(53).SetCellValue(protocol.HCLageri);
            rowHeader.CreateCell(54).SetCellValue("ХОДОВА ЧАСТ ЛАГЕРИ");
            rowData.CreateCell(54).SetCellValue(protocol.HCLageriDaNe);
            rowHeader.CreateCell(55).SetCellValue("ХОДОВА ЧАСТ СПИРАЧНА УРЕДБА");
            rowData.CreateCell(55).SetCellValue(protocol.HCSpirachnaUredba);
            rowHeader.CreateCell(56).SetCellValue("ХОДОВА ЧАСТ СПИРАЧНА УРЕДБА");
            rowData.CreateCell(56).SetCellValue(protocol.HCSpirachnaUredbaDaNe);
            rowHeader.CreateCell(57).SetCellValue("ХОДОВА ЧАСТ СТАБ. ЩАНГИ И ТАМПОНИ");
            rowData.CreateCell(57).SetCellValue(protocol.HCStabilizirashtiShtangiITamponi);
            rowHeader.CreateCell(58).SetCellValue("ХОДОВА ЧАСТ СТАБ. ЩАНГИ И ТАМПОНИ");
            rowData.CreateCell(58).SetCellValue(protocol.HCStabilizirashtiShtangiITamponiDaNe);
            rowHeader.CreateCell(59).SetCellValue("ХОДОВА ЧАСТ КОРМИЛНО УПРАВЛЕНИЕ");
            rowData.CreateCell(59).SetCellValue(protocol.HCKormilnoUpravlenie);
            rowHeader.CreateCell(60).SetCellValue("ХОДОВА ЧАСТ КОРМИЛНО УПРАВЛЕНИЕ");
            rowData.CreateCell(60).SetCellValue(protocol.HCKormilnoUpravlenieDaNe);
            rowHeader.CreateCell(61).SetCellValue("ХОДОВА ЧАСТ ГЕОМЕТРИЯ ПРЕДЕН И ЗАДЕН МОСТ");
            rowData.CreateCell(61).SetCellValue(protocol.HCGeometriaPredenIZadenMost);
            rowHeader.CreateCell(62).SetCellValue("ХОДОВА ЧАСТ ГЕОМЕТРИЯ ПРЕДЕН И ЗАДЕН МОСТ");
            rowData.CreateCell(62).SetCellValue(protocol.HCGeometriaPredenIZadenMostDaNe);
            rowHeader.CreateCell(63).SetCellValue("ХОДОВА ЧАСТ БЕАЛЕТИ");
            rowData.CreateCell(63).SetCellValue(protocol.HCBealeti);
            rowHeader.CreateCell(64).SetCellValue("ХОДОВА ЧАСТ БЕАЛЕТИ");
            rowData.CreateCell(64).SetCellValue(protocol.HCBealetiDaNe);
            rowHeader.CreateCell(65).SetCellValue("ХОДОВА ЧАСТ ДРУГИ");
            rowData.CreateCell(65).SetCellValue(protocol.HCDrugi);

            rowHeader.CreateCell(66).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА КЛИМАТИК");
            rowData.CreateCell(66).SetCellValue(protocol.ESKlimatik);
            rowHeader.CreateCell(67).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА КЛИМАТИК");
            rowData.CreateCell(67).SetCellValue(protocol.ESKlimatikDaNe);
            rowHeader.CreateCell(68).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА АКУМУЛАТОР");
            rowData.CreateCell(68).SetCellValue(protocol.ESAkumulator);
            rowHeader.CreateCell(69).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА АКУМУЛАТОР");
            rowData.CreateCell(69).SetCellValue(protocol.ESAkumulatorDaNe);
            rowHeader.CreateCell(70).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА СТАРТЕР И ГЕНЕРАТОР");
            rowData.CreateCell(70).SetCellValue(protocol.ESStarterIGenerator);
            rowHeader.CreateCell(69).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА СТАРТЕР И ГЕНЕРАТОР");
            rowData.CreateCell(69).SetCellValue(protocol.ESStarterIGeneratorDaNe);
            rowHeader.CreateCell(70).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА АРМАТУРНО ТАБЛО");
            rowData.CreateCell(70).SetCellValue(protocol.ESArmaturnoTablo);
            rowHeader.CreateCell(71).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА АРМАТУРНО ТАБЛО");
            rowData.CreateCell(71).SetCellValue(protocol.ESArmaturnoTabloDaNe);
            rowHeader.CreateCell(72).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА ЧИСТАЧКИ");
            rowData.CreateCell(72).SetCellValue(protocol.ESChistachki);
            rowHeader.CreateCell(73).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА ЧИСТАЧКИ");
            rowData.CreateCell(73).SetCellValue(protocol.ESChistachkiDaNe);
            rowHeader.CreateCell(74).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА СВЕТЛИНИ");
            rowData.CreateCell(74).SetCellValue(protocol.ESSvetlini);
            rowHeader.CreateCell(75).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА СВЕТЛИНИ");
            rowData.CreateCell(75).SetCellValue(protocol.ESSvetliniDaNe);
            rowHeader.CreateCell(76).SetCellValue("ЕЛЕКТРИЧЕСКА СИСТЕМА ДРУГИ");
            rowData.CreateCell(76).SetCellValue(protocol.ESDrugi);

            rowHeader.CreateCell(77).SetCellValue("ДРУГИ РЕКЛАМНА ТАБЕЛА");
            rowData.CreateCell(77).SetCellValue(protocol.DRReklamnaTabela);
            rowHeader.CreateCell(78).SetCellValue("ДРУГИ ВАЛИДНОСТ НА УТГ:");
            rowData.CreateCell(78).SetCellValue(protocol.DRValidnostUTG);
            rowHeader.CreateCell(79).SetCellValue("ИЗПРАВЕН");
            rowData.CreateCell(79).SetCellValue(protocol.AvtomobilatEIzpraven ? "ДА" : "");
            rowHeader.CreateCell(80).SetCellValue("НЕ ИЗПРАВЕН");
            rowData.CreateCell(80).SetCellValue(protocol.AvtomobilatENeIzpraven ? "ДА" : "");
            rowHeader.CreateCell(81).SetCellValue("МЕХАНИК");
            rowData.CreateCell(81).SetCellValue(protocol.Mehanik);
            rowHeader.CreateCell(82).SetCellValue("Вложени части");
            rowData.CreateCell(82).SetCellValue(protocol.VlojeniChasti);

            workbook.WriteExcelToResponse(HttpContext, fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task FromCellToExcel(string ToExcel)
        {
            var protodolId = Int32.Parse(ToExcel);
            ExportProtocol(protodolId);
        }

        [HttpGet]
        [Route("[controller]/[action]/{id}")]
        public IActionResult ExportToPDF(int id)
        {
            var protocolId = id;
            var protocol = _repoWrapper.Protocol.GetById(protocolId);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    ColorMode = ColorMode.Grayscale,
                    DocumentTitle = "ПРОТОКОЛ № " + protocol.ProtocolId +  protocol.Data.ToString(" ДАТА: dd-MM-yyyy")

                },

                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = CreateProtocolForPrinting(protocol),
                        WebSettings = { DefaultEncoding = "utf-8" },

                    }
                }
            };
            byte[] pdf = _converter.Convert(doc);

            return new FileContentResult(pdf, "application/pdf");
        }
        private string CreateProtocolForPrinting(Protocol protocol)
        {

            string print = @"<style>.border table, .border td, .border th{ border: 1px solid black;border-collapse: collapse;}</style><div style='font-size:26px;'><center>КОНСТАТИВЕН ПРОТОКОЛ № <strong>" + protocol.ProtocolId + "</strong> / <strong>" + protocol.Data.ToString("dd-MM-yyyy") + "</strong> г.</center></div>" +
                "<div style='font-size:16px;margin-top:50px;'><center>/<strong>протокола е валиден 3 (три) работни дни след издаването му</strong>/</center></div>" +
                "<div class='border' style='font-size:16px;margin:20px 0;'>" +
                "<table width='100%' style='font-size:18px'>" + "<tr><td>сл.№ <span style='font-size:22px;'><strong>" + protocol.SlujNomer + "</strong></span></td>" +
                "<td height='50'>рег. № <span style='font-size:22px;'><strong>" + protocol.RegNomer + "</strong></span></td>" +
                "<td>пробег: <span style='font-size:22px;'><strong>" + protocol.Probeg + "</strong></span> км</td>" +
                "<td>водач: <span style='font-size:22px;'><strong>" + protocol.Vodach.ToUpper() + "</strong></span></td></tr>" +
                "</table></div>" +
                "<div class='border'><table width='100%' style='font-size:18px'><tr><th>СИСТЕМИ</th><th>ОК</th><th>ЗАБЕЛЕЖКА</th></tr>" +
                "<tr><td width='50%'>Външен вид - Хигиена</td><td width='5%'>" + protocol.VVHigienaDaNe + "</td><td width='40%'>" + protocol.VVHigiena + "</td></tr>" +
                "<tr><td>Външен вид - Брони</td><td>" + protocol.VVBroniDaNe + "</td><td>" + protocol.VVBroni + "</td></tr>" +
                "<tr><td>Външен вид - Калници</td><td>" + protocol.VVKalniciDaNe + "</td><td>" + protocol.VVKalnici + "</td></tr>" +
                "<tr><td>Външен вид - Врати</td><td>" + protocol.VVVratiDaNe + "</td><td>" + protocol.VVVrati + "</td></tr>" +
                "<tr><td>Външен вид - Стъкла</td><td>" + protocol.VVStyklaDaNe + "</td><td>" + protocol.VVStykla + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Двигател</td><td>" + protocol.ZADvigatelDaNe + "</td><td>" + protocol.ZADvigatel + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Ремъци</td><td>" + protocol.ZaRemyciDaNe + "</td><td>" + protocol.ZaRemyci + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Охлаждаща уредба</td><td>" + protocol.ZAOhlajdashtaUredbaDaNe + "</td><td>" + protocol.ZAOhlajdashtaUredba + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Горивна уредба</td><td>" + protocol.ZAGorivnaUredbaDaNe + "</td><td>" + protocol.ZAGorivnaUredba + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Изпускателна уредба</td><td>" + protocol.ZAIzpuskatelnaUredbaDaNe + "</td><td>" + protocol.ZAIzpuskatelnaUredba + "</td></tr>" +
                "<tr><td>Задвижващ агрегат - Мазилна уредба</td><td>" + protocol.ZAMazilnaUredbaDaNe + "</td><td>" + protocol.ZAMazilnaUredba + "</td></tr>" +
                "<tr><td>Трансмисия - Съединител</td><td>" + protocol.TMSyedinitelDaNe + "</td><td>" + protocol.TMSyedinitel + "</td></tr>" +
                "<tr><td>Трансмисия - Скоростна кутия & диференциал</td><td>" + protocol.TMSkorostnaKutiqIDiferencialDaNe + "</td><td>" + protocol.TMSkorostnaKutiqIDiferencial + "</td></tr>" +
                "<tr><td>Ходова част - Ходови колела</td><td>" + protocol.HCHodoviKolelaDaNe + "</td><td>" + protocol.HCHodoviKolela + "</td></tr>" +
                "<tr><td>Ходова част - Тампони МФ</td><td>" + protocol.HCTamponiMFDaNe + "</td><td>" + protocol.HCTamponiMF + "</td></tr>" +
                "<tr><td>Ходова част - Амортисьори предни</td><td>" + protocol.HCAmortisioriPredniDaNe + "</td><td>" + protocol.HCAmortisioriPredni + "</td></tr>" +
                "<tr><td>Ходова част - Носачи предни</td><td>" + protocol.HCNosachiPredniDaNe + "</td><td>" + protocol.HCNosachiPredni + "</td></tr>" +
                "<tr><td>Ходова част - Шарнирни болтове</td><td>" + protocol.HCSharnirniBoltoveDaNe + "</td><td>" + protocol.HCSharnirniBoltove + "</td></tr>" +
                "<tr><td>Ходова част - Карета</td><td>" + protocol.HCKaretaDaNe + "</td><td>" + protocol.HCKareta + "</td></tr>" +
                "<tr><td>Ходова част - Маншони</td><td>" + protocol.HCManshoniDaNe + "</td><td>" + protocol.HCManshoni + "</td></tr>" +
                "<tr><td>Ходова част - Носачи задни</td><td>" + protocol.HCNosachiZadniDaNe + "</td><td>" + protocol.HCNosachiZadni + "</td></tr>" +
                "<tr><td>Ходова част - Амортисьори задни</td><td>" + protocol.HCAmortisioriZadniDaNe + "</td><td>" + protocol.HCAmortisioriZadni + "</td></tr>" +
                "<tr><td>Ходова част - Лагери</td><td>" + protocol.HCLageriDaNe + "</td><td>" + protocol.HCLageri + "</td></tr>" +
                "<tr><td>Ходова част - Спирачна уредба</td><td>" + protocol.HCSpirachnaUredbaDaNe + "</td><td>" + protocol.HCSpirachnaUredba + "</td></tr>" +
                "<tr><td>Ходова част - Стаб. щанги и т-ни</td><td>" + protocol.HCStabilizirashtiShtangiITamponiDaNe + "</td><td>" + protocol.HCStabilizirashtiShtangiITamponi + "</td></tr>" +
                "<tr><td>Ходова част - Кормилно управление</td><td>" + protocol.HCKormilnoUpravlenieDaNe + "</td><td>" + protocol.HCKormilnoUpravlenie + "</td></tr>" +
                "<tr><td>Ходова част - Геометрия преден и заден мост</td><td>" + protocol.HCGeometriaPredenIZadenMostDaNe + "</td><td>" + protocol.HCGeometriaPredenIZadenMost + "</td></tr>" +
                "<tr><td>Ходова част - Беалети</td><td>" + protocol.HCBealetiDaNe + "</td><td>" + protocol.HCBealeti + "</td></tr>" +
                "<tr><td>Ходова част - Други</td><td></td><td>" + protocol.HCDrugi + "</td></tr>" +
                "<tr><td>Ел. система - Климатик</td><td>" + protocol.ESKlimatikDaNe + "</td><td>" + protocol.ESKlimatik + "</td></tr>" +
                "<tr><td>Ел. система - Акумулатор</td><td>" + protocol.ESAkumulatorDaNe + "</td><td>" + protocol.ESAkumulator + "</td></tr>" +
                "<tr><td>Ел. система - Стартер и генератор</td><td>" + protocol.ESStarterIGeneratorDaNe + "</td><td>" + protocol.ESStarterIGenerator + "</td></tr>" +
                "<tr><td>Ел. система - Арматурно табло</td><td>" + protocol.ESArmaturnoTabloDaNe + "</td><td>" + protocol.ESArmaturnoTablo + "</td></tr>" +
                "<tr><td>Ел. система - Чистачки</td><td>" + protocol.ESChistachkiDaNe + "</td><td>" + protocol.ESChistachki + "</td></tr>" +
                "<tr><td>Ел. система - Светлини</td><td>" + protocol.ESSvetliniDaNe + "</td><td>" + protocol.ESSvetlini + "</td></tr>" +
                "<tr><td>Други - Рекламна табела</td><td></td><td>" + protocol.DRReklamnaTabela + "</td></tr>" +
                "<tr><td>Вложени части</td><td></td><td>" + protocol.VlojeniChasti + "</td></tr>" +
                "</table>" +
                "</div>" +
                "<div style='margin-top:40px;'>" +
                "<table width='100%' style='font-size:18px;font-weight:bold;'" +
                "<tr><td width='50%'>водач:</td><td>изготвил:</td></tr>" +
                "<tr><td><div style='text-indent:70px;'>/ " + protocol.Vodach + " /</div></td><td></td></tr>" +
                "</div>";
            return print;
        }
    }
}
