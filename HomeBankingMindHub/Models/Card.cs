using HomeBankingMindHub.Models.utils;

namespace HomeBankingMindHub.Models
{
    public class Card
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public CardType Type { get; set; }
        public ColorType Color { get; set; }
        public string Number { get; set; }
        public int CVV { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ThruDate { get; set; }
        public long ClientId { get; set; }
        public Client Client { get; set; }
        
    }
}
