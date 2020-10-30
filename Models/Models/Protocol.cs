
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models
{
    public class Protocol
    {
        [Key]
        public int ProtocolId { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime Data { get; set; }
        [Range(1, Int32.MaxValue, ErrorMessage = "Не валиден \"Служебен №\".")]
        [Required(ErrorMessage = "Полето \"Служебен №\" е задължително.")]
        public int? SlujNomer { get; set; }
        public string RegNomer { get; set; }
        [Required(ErrorMessage = "Полето \"Пробег\" е задължително.")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Не валиден \"Пробег\".")]
        public int? Probeg { get; set; }
        [Required(ErrorMessage = "Полето \"Водач\" е задължително.")]
        public string Vodach { get; set; }
        public string VVHigiena { get; set; }
        public string VVHigienaDaNe { get; set; }
        public string VVBroni { get; set; }
        public string VVBroniDaNe { get; set; }
        public string VVKalnici { get; set; }
        public string VVKalniciDaNe { get; set; }
        public string VVVrati { get; set; }
        public string VVVratiDaNe { get; set; }
        public string VVStykla { get; set; }
        public string VVStyklaDaNe { get; set; }
        public string VVDrugi { get; set; }
        public string ZADvigatel { get; set; }
        public string ZADvigatelDaNe { get; set; }
        public string ZaRemyci { get; set; }
        public string ZaRemyciDaNe { get; set; }
        public string ZAOhlajdashtaUredba { get; set; }
        public string ZAOhlajdashtaUredbaDaNe { get; set; }
        public string ZAGorivnaUredba { get; set; }
        public string ZAGorivnaUredbaDaNe { get; set; }
        public string ZAIzpuskatelnaUredba { get; set; }
        public string ZAIzpuskatelnaUredbaDaNe { get; set; }
        public string ZAMazilnaUredba { get; set; }
        public string ZAMazilnaUredbaDaNe { get; set; }
        public string ZADrugi { get; set; }
        public string TMSyedinitel { get; set; }
        public string TMSyedinitelDaNe { get; set; }
        public string TMSkorostnaKutiqIDiferencial { get; set; }
        public string TMSkorostnaKutiqIDiferencialDaNe { get; set; }
        public string TMDrugi { get; set; }
        public string HCHodoviKolela { get; set; }
        public string HCHodoviKolelaDaNe { get; set; }
        public string HCTamponiMF { get; set; }
        public string HCTamponiMFDaNe { get; set; }
        public string HCAmortisioriPredni { get; set; }
        public string HCAmortisioriPredniDaNe { get; set; }
        public string HCNosachiPredni { get; set; }
        public string HCNosachiPredniDaNe { get; set; }
        public string HCSharnirniBoltove { get; set; }
        public string HCSharnirniBoltoveDaNe { get; set; }
        public string HCKareta { get; set; }
        public string HCKaretaDaNe { get; set; }
        public string HCManshoni { get; set; }
        public string HCManshoniDaNe { get; set; }
        public string HCNosachiZadni { get; set; }
        public string HCNosachiZadniDaNe { get; set; }
        public string HCAmortisioriZadni { get; set; }
        public string HCAmortisioriZadniDaNe { get; set; }
        public string HCLageri { get; set; }
        public string HCLageriDaNe { get; set; }
        public string HCSpirachnaUredba { get; set; }
        public string HCSpirachnaUredbaDaNe { get; set; }
        public string HCStabilizirashtiShtangiITamponi { get; set; }
        public string HCStabilizirashtiShtangiITamponiDaNe { get; set; }
        public string HCKormilnoUpravlenie { get; set; }
        public string HCKormilnoUpravlenieDaNe { get; set; }
        public string HCGeometriaPredenIZadenMost { get; set; }
        public string HCGeometriaPredenIZadenMostDaNe { get; set; }
        public string HCBealeti { get; set; }
        public string HCBealetiDaNe { get; set; }
        public string HCDrugi { get; set; }
        public string ESKlimatik { get; set; }
        public string ESKlimatikDaNe { get; set; }
        public string ESAkumulator { get; set; }
        public string ESAkumulatorDaNe { get; set; }
        public string ESStarterIGenerator { get; set; }
        public string ESStarterIGeneratorDaNe { get; set; }
        public string ESArmaturnoTablo { get; set; }
        public string ESArmaturnoTabloDaNe { get; set; }
        public string ESChistachki { get; set; }
        public string ESChistachkiDaNe { get; set; }
        public string ESSvetlini { get; set; }
        public string ESSvetliniDaNe { get; set; }
        public string ESDrugi { get; set; }
        public string DRReklamnaTabela { get; set; }
        public string DRValidnostUTG { get; set; }
        public bool AvtomobilatEIzpraven { get; set; }
        public bool AvtomobilatENeIzpraven { get; set; }
        [Required(ErrorMessage = "Полето \"Механик\" е задължително.")]
        public string Mehanik { get; set; }
        public string VlojeniChasti { get; set; }
        public bool IsBlock { get; set; }
        public bool IsExternal { get; set; }
        public bool IsTransfered { get; set; }

    }
}





