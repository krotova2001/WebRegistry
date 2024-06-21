using System.Globalization;

namespace WebRegistry.Services.DraftConverter
{

    //класс чертежа. Это все данные, считанные с одного чертежа
    public class Draft
    {
        public Draft() { }
        public string Shifr { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string Jx { get; set; } = string.Empty;
        public string Wx { get; set; } = string.Empty;
        public string Jy { get; set; } = string.Empty;
        public string Wy { get; set; } = string.Empty;
        public string Square { get; set; } = string.Empty;
        public string Weight { get; set; } = string.Empty;
        public string Perimeter { get; set; } = string.Empty;
        public string CircleDiametr { get; set; } = string.Empty;
        public string DifficultyGroup { get; set; } = string.Empty;
        public string GOST { get; set; } = string.Empty;
        public string Razrab { get; set; } = string.Empty;
        public string Prov { get; set; } = string.Empty;
        public string TKontr { get; set; } = string.Empty;
        public string NKontr { get; set; } = string.Empty;
        public string Master { get; set; } = string.Empty;
        public string Utverd { get; set; } = string.Empty;
        public string CustomerShifr { get; set; } = string.Empty;
        public string WeightAL { get; set; } = string.Empty;
        public int ContainExtComponent { get; set; } = 0;
        public int ContainDetails { get; set; } = 0;
        public string Note { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;

    }


    public class DraftDTO
    {
        private IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
        public DraftDTO() { }
        public DraftDTO(Draft draft)
        {
            Shifr = draft.Shifr;
            FilePath = draft.FilePath;
            if (draft.Jx != string.Empty)
                Jx = double.Parse(draft.Jx, formatter);
            if (draft.Wx != string.Empty)
                Wx = double.Parse(draft.Wx, formatter);
            if (draft.Jy != string.Empty)
                Jy = double.Parse(draft.Jy, formatter);
            if (draft.Wy != string.Empty)
                Wy = double.Parse(draft.Wy, formatter);
            if (draft.Square != string.Empty)
                Square = double.Parse(draft.Square, formatter);
            try
            {
                Weight = double.Parse(draft.Weight, formatter);
            }
            catch
            {
                Weight = 0;
            }

            if (draft.Perimeter != string.Empty)
                Perimeter = double.Parse(draft.Perimeter, formatter);
            if (draft.CircleDiametr != string.Empty)
            {
                var res = draft.CircleDiametr.Trim('.');
                CircleDiametr = double.Parse(res, formatter);
            }
            DifficultyGroup = draft.DifficultyGroup;
            GOST = draft.GOST;
            Razrab = draft.Razrab;
            Prov = draft.Prov;
            TKontr = draft.TKontr;
            NKontr = draft.NKontr;
            Master = draft.Master;
            Utverd = draft.Utverd;
            CustomerShifr = draft.CustomerShifr;
            if (draft.WeightAL != string.Empty)
                WeightAL = double.Parse(draft.WeightAL, formatter);
            ContainExtComponent = draft.ContainExtComponent;
            ContainDetails = draft.ContainDetails;
            Note = draft.Note;
            CustomerName = draft.CustomerName;
        }
        public string Shifr { get; set; }
        public string FilePath { get; set; }
        public double Jx { get; set; }
        public double Wx { get; set; }
        public double Jy { get; set; }
        public double Wy { get; set; }
        public double Square { get; set; }
        public double Weight { get; set; }
        public double Perimeter { get; set; }
        public double CircleDiametr { get; set; }
        public string DifficultyGroup { get; set; }
        public string GOST { get; set; }
        public string Razrab { get; set; }
        public string Prov { get; set; }
        public string TKontr { get; set; }
        public string NKontr { get; set; }
        public string Master { get; set; }
        public string Utverd { get; set; }
        public string CustomerShifr { get; set; }
        public double WeightAL { get; set; }
        public int ContainExtComponent { get; set; } = 0;
        public int ContainDetails { get; set; } = 0;
        public string Note { get; set; } = string.Empty;
        public string CustomerName { get; set; }

    }

    //класс для трансляции временной таблицы - Детали
    public class TempLinks
    {
        public string ParentShifr { get; set; } = string.Empty;
        public string Shifr { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gost { get; set; } = string.Empty;
        public string Count { get; set; }
        public string Weight { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }

    //покупные изделия
    public class ExtComponent
    {
        public string ParentShifr { get; set; } = string.Empty;
        public string Shifr { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Count { get; set; }
        public string Weight { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}

