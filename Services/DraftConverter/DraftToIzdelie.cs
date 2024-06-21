using Newtonsoft.Json;
using WebRegistry.Models;
using WebRegistry.Services.ShifrRezerver;

namespace WebRegistry.Services.DraftConverter
{
    //класс, транслирующий чертеж Draft в модель Izdelie
    public class DraftToIzdelie
    {
        private readonly IShifrService _shifrService;
        private Izdelie _izdelie;
        WebDwgParser textParser;
        private DraftDTO _draft;

        //варинат на случай, если библиотека парсинга чертежей не работает временно
        public DraftToIzdelie(IShifrService shifrService) {
            _shifrService = shifrService;
        }
        public DraftToIzdelie(string filename, IShifrService shifrService) 
        {
            _shifrService = shifrService;

            //переделать на внешний проект
            textParser = new WebDwgParser(filename);
            try
            {
                _draft = new DraftDTO(textParser.Parse());
                _izdelie = JsonConvert.DeserializeObject<Izdelie>(JsonConvert.SerializeObject(_draft));
            }
            catch (Exception  EX_NAME)
            {
                Console.WriteLine(EX_NAME);
                _draft = new DraftDTO();
            }
           
        }

        public Izdelie GetNewIzdelie 
        {
            get
            {
                if (_izdelie == null)
                    _izdelie= new Izdelie();
                
                //сгенерировать новый шифр
                if (_izdelie.Shifr == String.Empty)
                {
                    _izdelie.Shifr = _shifrService.GetNewShifr();
                }
                return _izdelie;
            }
        }
    }
}
