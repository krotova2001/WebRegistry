using WebRegistry.Models;

namespace WebRegistry.Services.ShifrRezerver
{
    public class ShifrWorker : IShifrService
    {
        private readonly NomenclatureContext _context;
        private string[] _allShifres;
        private readonly int[] _allInvNumbers;
        public ShifrWorker(NomenclatureContext context) 
        {
            _context = context;

            //нужно собрать шифры с изделий и с архива
            string[] Shifres = _context.Izdelies.Select(s => s.Shifr).ToArray();
            string[] shifresFromRedbook = _context.RedBookArchive.Select(r => r.ShifrRedBook).ToArray();
            _allShifres = Shifres.Concat(shifresFromRedbook).ToArray();
            _allInvNumbers = _context.RedBookArchive.Select(n => n.Inventory).ToArray();
        }
        public string GetNewShifr()
        {
            return ShifrGenerator.GetNextShifr(_allShifres);
        }

        public bool IsShifrExist(string shifr)
        {
            string query = shifr.Trim().ToUpper().Trim(' ');
            return _allShifres.Any(s => s.Equals(query));
        }

        public bool RezerveShifr(string shifr)
        {
            throw new NotImplementedException();
        }

        public int GetNewInvNumber()
        {
            int max = _allInvNumbers.Max();
            ++max;
            return max;
        }

        public bool IsInvNumberExist(string number)
        {
            //исправить если строка не парсится
            int res = -1;
            if (int.TryParse(number, out res))
            {
                return _allInvNumbers.Any(s => s == res);
            }
            return false;
        }
    }
}
