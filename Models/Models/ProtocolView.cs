using System.Collections.Generic;

namespace Models
{
    public class ProtocolView : Protocol
    {
        public ProtocolView()
        {
            VVBroniOption = new List<VVBroniOption> {
               new VVBroniOption { VVBroniOptionId = 1, VVBroniOptionValue = "ПРЕДНА" },
               new VVBroniOption { VVBroniOptionId = 2, VVBroniOptionValue = "ЗАДНА" }};

            VVKalniciOption = new List<VVKalniciOption> {
               new VVKalniciOption {VVKalniciOptionId = 1, VVKalniciOptionValue = "ЛЯВ" },
               new VVKalniciOption { VVKalniciOptionId = 2, VVKalniciOptionValue = "ДЕСЕН" },
                new VVKalniciOption {VVKalniciOptionId = 3, VVKalniciOptionValue = "ПРЕДЕН" },
                new VVKalniciOption {VVKalniciOptionId = 4, VVKalniciOptionValue = "ЗАДЕН" }};

            VVVratiOption = new List<VVVratiOption> {
               new VVVratiOption {VVVratiOptionId = 1, VVVratiOptionValue = "ЛЯВА ПРЕДНА" },
               new VVVratiOption { VVVratiOptionId = 2, VVVratiOptionValue = "ЛЯВА ЗАДНА" },
                new VVVratiOption {VVVratiOptionId = 3, VVVratiOptionValue = "ДЯСНА ПРЕДНА" },
                new VVVratiOption {VVVratiOptionId = 4, VVVratiOptionValue = "ДЯСНА ЗАДНА" }};

            VVStyklaOption = new List<VVStyklaOption> {
                  new VVStyklaOption {VVStyklaOptionId = 1,VVStyklaOptionValue = "ЧЕЛНО" },
               new VVStyklaOption {VVStyklaOptionId = 2,VVStyklaOptionValue = "СТРАНИЧНО" },
                new VVStyklaOption {VVStyklaOptionId = 3,VVStyklaOptionValue = "ЗАДНО" }};

            VVDrugiOption = new List<VVDrugiOption> {
                        new VVDrugiOption {VVDrugiOptionId = 1,VVDrugiOptionValue = "" },
                       new VVDrugiOption {VVDrugiOptionId = 2,VVDrugiOptionValue = "ПРАГ" }};
        }
        public IEnumerable<VVBroniOption> VVBroniOption { get; private set; }
        public IEnumerable<VVKalniciOption> VVKalniciOption { get; private set; }
        public IEnumerable<VVVratiOption> VVVratiOption { get; private set; }
        public IEnumerable<VVStyklaOption> VVStyklaOption { get; private set; }
        public IEnumerable<VVDrugiOption> VVDrugiOption { get; private set; }
        public IEnumerable<Place> Place { get; set; }
        public int PlaceId { get; set; }
        public int QueueId { get; set; }
        public string OptionName { get; set; }
        public bool InLine { get; set; }
    }
}