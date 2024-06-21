using CADImport;


namespace WebRegistry.Services.DraftConverter
{
    public class WebDwgParser
    {
        public static int subDraftcount = 0;
        private string file;
        private Draft draft;
        private List<Draft> Drafts;
        public List<TempLinks> tempLinksLst;
        public List<ExtComponent> extComponentLst;
        //пограничные точки для поиска 
        private Point Oboznach, Naimenov, Col, Massa, Prim, Massa1pmO, MassAL, Izdeliz, Razrab, Square;
        private bool parse_jjww_fromtext = true;
        private string Shifr;
        private bool Naves = false; // отмка, что это навесное изделие
        private double RightPointX = 0.0; //правая граница всего по оси X
        public bool isManyDrafts = false;
        private CADImage cadImage;

        private List<Point> headers;
        private List<Point> bottoms;

        public WebDwgParser(string filename, bool naves = false)
        {
            file = filename;
            draft = new Draft();
            tempLinksLst = new List<TempLinks>();
            extComponentLst = new List<ExtComponent>();
            Naves = naves;
            try
            {
                cadImage = CADImage.CreateImageByExtension(file);
                cadImage.LoadFromFile(file);
            }
            catch (Exception e)
            {
                return;
            }
           
            

            //набор данных для нескольких чртежей

            headers = new List<Point>();
            bottoms = new List<Point>();
            for (int i = 0; i < cadImage.CurrentLayout.Count; i++)
            {
                if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.Text)
                {
                    CADText vText = (CADText)cadImage.CurrentLayout.Entities[i];
                    if (vText.Text != string.Empty)
                    {
                        if (vText.Rotation == 180 && vText.Text.Length > 2)
                        {
                            headers.Add(new Point((int)vText.Point.X, (int)vText.Point.Y));
                        }
                        if (vText.UnicodeText.ToLower().Contains("масштаб"))
                        {
                            bottoms.Add(new Point((int)vText.Point1.X, (int)vText.Point.Y - 40));
                        }

                    }
                }
            }

            if (headers.Count > 1)
            {
                isManyDrafts = true;
                Console.WriteLine("Несколько чертежей");
                subDraftcount++;
            }


        }

        public List<Draft> ParseMany()
        {
            Drafts = new List<Draft>();
            return Drafts;
        }


        public Draft Parse()
        {
            draft.FilePath = file;
            Console.WriteLine(Path.GetFileName(file) + " - " + file);
            Console.WriteLine("--------------------");
            Point key = new Point();
            List<CADText> texttable = new List<CADText>();

            for (int i = 0; i < cadImage.CurrentLayout.Count; i++)
            {
                if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.Text)
                {
                    CADText vText = (CADText)cadImage.CurrentLayout.Entities[i];
                    if (vText.Text != string.Empty)
                    {
                        if (Naves && RightPointX != 0.0)
                        {
                            if (vText.Point.X < RightPointX)
                                texttable.Add(vText);
                        }
                        else
                        {
                            texttable.Add(vText);
                        }
                    }
                }
            }


            for (int i = 0; i < cadImage.CurrentLayout.Count; i++)
            {
                if (!Naves) // у навесных изделий нет моментов иннерции
                {
                    //вставки коэфф жесткости
                    if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.Insert)
                    {
                        CADInsert cADInsert = (CADInsert)cadImage.CurrentLayout.Entities[i];
                        if (cADInsert.Attribs.Count > 0)
                        {
                            for (int j = 0; j < cADInsert.Attribs.Count; j++)
                            {
                                CADAttrib attrib = (CADAttrib)cADInsert.Attribs[j];
                                switch (j)
                                {
                                    case 0:
                                        Console.Write("J(x) - ");
                                        draft.Jx = attrib.UnicodeText;
                                        break;
                                    case 1:
                                        Console.Write("W(x) - ");
                                        draft.Wx = attrib.UnicodeText;
                                        break;
                                    case 2:
                                        Console.Write("J(y) - ");
                                        draft.Jy = attrib.UnicodeText;
                                        break;
                                    case 3:
                                        Console.Write("W(y) - ");
                                        draft.Wy = attrib.UnicodeText;
                                        break;
                                    default:
                                        break;
                                }

                                Console.WriteLine(attrib.UnicodeText);
                                parse_jjww_fromtext = false;
                            }
                        }

                    }
                }


                if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.Text)
                {
                    CADText vText = (CADText)cadImage.CurrentLayout.Entities[i];
                    if (vText.Text != string.Empty)
                    {
                        key.X = (int)vText.StartPoint.X;
                        key.Y = (int)vText.StartPoint.Y;

                        if (vText.Rotation == 180 && vText.Text.Length > 2)
                        {

                            //шифр у навесных в случае несколких чертежей берем самый левый
                            if (Naves && RightPointX != 0.0)
                            {
                                if (vText.Point.X < RightPointX)
                                {
                                    Shifr = vText.UnicodeText != string.Empty ? vText.UnicodeText : vText.Text;
                                    Console.WriteLine("Шифр - " + Shifr);
                                    draft.Shifr = Shifr;
                                }
                            }

                            else
                            {
                                Shifr = vText.UnicodeText != string.Empty ? vText.UnicodeText : vText.Text;
                                Console.WriteLine("Шифр - " + Shifr);
                                draft.Shifr = Shifr;
                            }
                        }


                        if (vText.UnicodeText.Contains("Площадь") ||
                            vText.UnicodeText.Contains("Масса 1") ||
                            vText.UnicodeText.Contains("Периметр") ||
                            vText.UnicodeText.Contains("Диаметр") ||
                            vText.UnicodeText.Contains("Группа") ||
                            vText.UnicodeText.ToLower().Contains("шифр заказчика") ||
                            vText.UnicodeText.ToLower() == "масса"
                            )
                        {

                            //ищем значение которое примерно снизу
                            for (int y = 0; y < cadImage.CurrentLayout.Count; y++)
                            {
                                if (cadImage.CurrentLayout.Entities[y].EntType == EntityType.Text)
                                {
                                    CADText sText = (CADText)cadImage.CurrentLayout.Entities[y];
                                    if (sText.Text != string.Empty)
                                    {
                                        if (sText.StartPoint.X <= key.X + 10 && sText.StartPoint.X >= key.X - 10)
                                        {
                                            if (sText.StartPoint.Y >= key.Y - 18 && sText.StartPoint.Y <= key.Y - 8)
                                            {
                                                if (sText.Text != string.Empty)
                                                {
                                                    Console.WriteLine(vText.UnicodeText + " - " + sText.UnicodeText);

                                                    //тупизм)))
                                                    if (vText.UnicodeText.Contains("Площадь"))
                                                    {
                                                        draft.Square = sText.UnicodeText;
                                                        Square = new Point { X = (int)sText.Point.X, Y = (int)sText.Point.Y };
                                                    }
                                                    if (vText.UnicodeText.Contains("Масса 1"))
                                                        draft.Weight = sText.UnicodeText;
                                                    if (vText.UnicodeText.Contains("Периметр"))
                                                        draft.Perimeter = sText.UnicodeText;
                                                    if (vText.UnicodeText.Contains("Диаметр"))
                                                        draft.CircleDiametr = sText.UnicodeText;
                                                    if (vText.UnicodeText.Contains("Группа"))
                                                        draft.DifficultyGroup = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("шифр заказчика"))
                                                        draft.CustomerShifr = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower() == "масса")
                                                    {
                                                        if (!vText.UnicodeText.ToLower().Contains("таблицу"))
                                                            draft.Weight = sText.UnicodeText;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }

                                /*
                                if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.MText)
                                {

                                    string S = string.Empty;
                                    CADMText vMText = (CADMText)cadImage.CurrentLayout.Entities[i];
                                    for (int j = 0; j < vMText.Block.Count; j++)
                                    {
                                        {
                                            Console.WriteLine(vMText.Text + "!!!!!!!!!!!!!! " + vMText.Point.X + "/" + vMText.Point.Y);
                                        }
                                    }
                                }
                                */

                            }
                        }
                        //Фамилии
                        if (vText.UnicodeText.ToLower().Contains("разраб.")
                            || vText.UnicodeText.ToLower().Contains("пров.")
                            || vText.UnicodeText.ToLower().Contains("контр.")
                            || vText.UnicodeText.ToLower().Contains("мастер")
                            || vText.UnicodeText.ToLower().Contains("утв.")
                            )
                        {
                            if (vText.UnicodeText.ToLower().Contains("разраб."))
                                Razrab = new Point { X = (int)vText.Point.X, Y = (int)vText.Point.Y };
                            //ищем значение которое примерно справа
                            for (int y = 0; y < cadImage.CurrentLayout.Count; y++)
                            {
                                if (cadImage.CurrentLayout.Entities[y].EntType == EntityType.Text)
                                {
                                    CADText sText = (CADText)cadImage.CurrentLayout.Entities[y];
                                    if (sText.Text != string.Empty)
                                    {
                                        if (sText.StartPoint.X < key.X + 20 && sText.StartPoint.X > key.X + 15)
                                        {
                                            if (sText.StartPoint.Y < key.Y + 2 && sText.StartPoint.Y > key.Y - 2)
                                            {
                                                if (sText.Text != string.Empty)
                                                {
                                                    Console.WriteLine(vText.UnicodeText + " - " + sText.UnicodeText);
                                                    if (vText.UnicodeText.ToLower().Contains("разраб."))
                                                        draft.Razrab = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("пров."))
                                                        draft.Prov = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("н.контр."))
                                                        draft.NKontr = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("мастер"))
                                                        draft.Master = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("утв."))
                                                        draft.Utverd = sText.UnicodeText;
                                                    if (vText.UnicodeText.ToLower().Contains("т.контр."))
                                                        draft.TKontr = sText.UnicodeText;
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }


                    }
                }
            }


            foreach (var sText in texttable)
            {
                //маркировка ГОСТ
                if (sText.UnicodeText.ToLower().Contains("гост") && !sText.UnicodeText.ToLower().Contains("условия"))
                {
                    if (sText.Point.X > Razrab.X && sText.Point.Y < Razrab.Y)
                    {
                        Console.WriteLine(sText.UnicodeText);
                        draft.GOST = sText.UnicodeText;
                    }
                }

                //примечания
                if (Naves)
                {
                    if (sText.UnicodeText.ToLower().Contains("допускается"))
                    {
                        draft.Note += sText.UnicodeText.Trim('"').Trim();
                    }
                    if (sText.UnicodeText.ToLower().Contains("маркировать"))
                    {
                        draft.Note += sText.UnicodeText.Trim().Trim('"');
                    }

                }
            }

            foreach (var sText in texttable)
            {
                if (sText.Text.Contains("Сплав"))
                {
                    ParseSplavTable(texttable, sText);
                }
            }

            Console.WriteLine();
            ParseCustomer(texttable);
            ParseContainedDetails(texttable);

            if (!Naves)
            {
                if (parse_jjww_fromtext)
                    ParseJJWW(texttable);
            }

            foreach (var det in tempLinksLst)
                det.ParentShifr = draft.Shifr;
            foreach (var etx in extComponentLst)
                etx.ParentShifr = draft.Shifr;

            cadImage.Dispose();

            return draft;

        }
        private void ParseCustomer(List<CADText> texttable)
        {
            foreach (var sText in texttable)
            {
                if (sText.UnicodeText.ToLower() == "заказчик")
                {
                    foreach (var vText in texttable)
                    {
                        if (vText.StartPoint.X < sText.StartPoint.X + 4 && vText.StartPoint.X >= sText.StartPoint.X - 8)
                        {
                            if (vText.StartPoint.Y <= sText.StartPoint.Y - 8 && vText.StartPoint.Y >= sText.StartPoint.Y - 12)
                            {
                                Console.WriteLine($"Заказчик - {vText.UnicodeText}");
                                draft.CustomerName = vText.UnicodeText;
                            }
                        }
                    }
                }
            }
        }

        //взять инфу из составного чертежа
        private void ParseContainedDetails(List<CADText> texttable)
        {
            List<string> texts = texttable.Select(x => x.UnicodeText).ToList();
            Point Details = new Point();

            if (texts.Contains("Обозначение") && texts.Contains("Наименование"))
            {
                foreach (var vText in texttable)
                {
                    //Console.WriteLine($"{vText.UnicodeText}");
                    //Заполнить координаты границ
                    if (vText.UnicodeText.ToLower().Contains("обозначение"))
                        Oboznach = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                    if (vText.UnicodeText.ToLower().Contains("масса 1 п.м. общая") || vText.Text.ToLower().Contains("масса 1 п.м. общая"))
                        Massa1pmO = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                    if (vText.EntName.ToLower().Contains("изделия"))
                        Izdeliz = new Point { X = (int)vText.Point.X, Y = (int)vText.Point.Y };
                    if (vText.UnicodeText.ToLower().Contains("масса 1 п.м. al"))
                        MassAL = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };

                    if (vText.UnicodeText.Contains("Наименование"))
                        Naimenov = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                    if (vText.UnicodeText.ToLower().Contains("кол"))
                        Col = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };

                    //только верхняя строчка - заголовки таблицы
                    if (vText.StartPoint.X > Oboznach.X && vText.StartPoint.Y > Oboznach.Y - 10)
                    {
                        if (vText.UnicodeText.Contains("Наименование"))
                            Naimenov = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                        if (vText.UnicodeText.ToLower().Contains("кол"))
                            Col = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                        if (vText.UnicodeText.ToLower().Contains("масса"))
                            Massa = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                        if (vText.UnicodeText.ToLower().Contains("прим"))
                            Prim = new Point { X = (int)vText.StartPoint.X, Y = (int)vText.StartPoint.Y };
                    }
                }

                Console.WriteLine("\nСоставной чертеж" + "\n-----------------");

                foreach (var vText in texttable)
                {

                    //Console.WriteLine($"{vText.EntName} - {vText.Point.X}");
                    //масса алюминия
                    if (vText.UnicodeText.ToLower().Contains("масса 1 п.м. al"))
                    {
                        foreach (var sText in texttable)
                        {
                            if (sText.StartPoint.X <= MassAL.X + 60 && sText.StartPoint.X >= MassAL.X + 50)
                            {
                                if (sText.StartPoint.Y <= MassAL.Y + 2 && sText.StartPoint.Y >= MassAL.Y - 2)
                                {
                                    if (sText.Text != string.Empty)
                                    {
                                        Console.WriteLine(vText.UnicodeText + " - " + sText.UnicodeText + "\n");
                                        draft.WeightAL = sText.UnicodeText;
                                    }
                                }
                            }
                        }
                    }

                    if (vText.EntName.ToLower().Contains("изделия"))
                    {
                        Point Izdeliz2 = new Point { X = (int)vText.Point.X, Y = (int)vText.Point.Y };
                        //  Izdeliz.X = (int)vText.Point.X; Izdeliz.Y = (int)vText.Point.Y;
                        Console.WriteLine("Покупные изделия");

                        //у навесных другие границы
                        if (Naves && RightPointX > 0)
                        {
                            foreach (var sText in texttable)
                            {
                                if (sText.StartPoint.X <= Izdeliz2.X - 40 && sText.StartPoint.X >= Izdeliz2.X - 50)
                                {
                                    if (sText.StartPoint.Y <= Izdeliz2.Y && sText.StartPoint.Y >= Izdeliz2.Y - 35)
                                    {
                                        //Console.WriteLine(sText.UnicodeText);
                                        ReadLine(texttable, sText);
                                        draft.ContainExtComponent = 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (var sText in texttable)
                            {
                                if (sText.StartPoint.X <= Izdeliz2.X - 47 && sText.StartPoint.X >= Izdeliz2.X - 55)
                                {
                                    if (Massa1pmO.IsEmpty)
                                        Massa1pmO = Square;
                                    if (sText.StartPoint.Y >= Massa1pmO.Y && sText.StartPoint.Y <= Izdeliz2.Y)
                                    {
                                        //Console.WriteLine(sText.UnicodeText);
                                        ReadLine(texttable, sText);
                                        draft.ContainExtComponent = 1;
                                    }
                                }
                            }
                        }
                        Console.WriteLine("");
                    }

                    if (vText.EntName.ToLower().Contains("детали"))
                    {
                        Console.WriteLine("Детали");
                        Details.X = (int)vText.Point.X;
                        Details.Y = (int)vText.Point.Y;
                        draft.ContainDetails = 1;
                        //у навесных другие границы
                        if (Naves && RightPointX > 0)
                        {
                            foreach (var sText in texttable)
                            {
                                if (sText.StartPoint.X <= Details.X - 45 && sText.StartPoint.X >= Details.X - 50)
                                {
                                    if (sText.StartPoint.Y > Izdeliz.Y && sText.StartPoint.Y < Details.Y)
                                    {
                                        //Console.WriteLine(sText.UnicodeText);
                                        ReadLine(texttable, sText, true);
                                    }
                                }
                            }
                        }

                        else
                        {
                            //поиск Деталей
                            foreach (var sText in texttable)
                            {
                                if (sText.StartPoint.X <= Details.X - 60 && sText.StartPoint.X >= Details.X - 66)
                                {
                                    if (sText.StartPoint.Y >= MassAL.Y && sText.StartPoint.Y <= Details.Y)
                                    {
                                        //Console.WriteLine(sText.UnicodeText);
                                        ReadLine(texttable, sText, true);
                                    }
                                }
                            }
                        }

                        Console.WriteLine("");
                    }


                }
            }
        }

        //взять таблицу сплавов
        private void ParseSplavTable(List<CADText> texttabl, CADText splav)
        {
            Console.WriteLine("\nМасса в зависимости от сплава");
            List<CADText> Headers = new List<CADText>();
            List<CADText> rows = new List<CADText>();
            Headers.Add(splav);

            AddInRow(texttabl, splav, Headers);

            foreach (var sText in Headers)
            {
                //AddInColmn(texttabl, sText, rows);
            }
        }

        //прочитать строку текста справа от образца
        private void ReadLine(List<CADText> texttable, CADText example, bool addGost = false)
        {
            TempLinks tempLinks = null;
            ExtComponent extComponent = null;

            Dictionary<string, Point> points = new Dictionary<string, Point>();
            points.Add("Обозначение", Oboznach);
            points.Add("Примечание", Prim);
            points.Add("Количество", Col);
            points.Add("Наименование", Naimenov);
            points.Add("Масса 1 п.м.", Massa);

            Console.Write("Обозначение - " + example.UnicodeText);
            if (addGost)
            {
                tempLinks = new TempLinks();
                tempLinks.Shifr = example.UnicodeText;
            }

            if (!addGost)
            {
                extComponent = new ExtComponent();
                extComponent.Shifr = example.UnicodeText;
            }

            foreach (var sText in texttable)
            {
                //берем то, что правее фразы
                if (sText.StartPoint.X > example.StartPoint.X && sText.StartPoint.X < example.StartPoint.X + 165)
                {
                    if (sText.StartPoint.Y >= example.StartPoint.Y - 2 && sText.StartPoint.Y <= example.StartPoint.Y + 2)
                    {
                        foreach (var p in points)
                        {
                            //берем то, что чуть левее или правее заголовка
                            if (sText.StartPoint.X >= p.Value.X - 6 && sText.StartPoint.X <= p.Value.X + 4)
                            {
                                Console.Write($"; {p.Key} - {sText.UnicodeText}");

                                //заполнение Деталей
                                if (addGost && tempLinks != null)
                                {
                                    if (p.Key == "Количество")
                                        tempLinks.Count = sText.UnicodeText;
                                    if (p.Key == "Масса 1 п.м.")
                                        tempLinks.Weight = sText.UnicodeText;
                                    if (p.Key == "Примечание")
                                        tempLinks.Note = sText.UnicodeText;
                                    if (p.Key == "Наименование")
                                        tempLinks.Name = sText.UnicodeText;
                                }

                                //заполнение полкупных изделий
                                if (!addGost)
                                {
                                    if (p.Key == "Количество")
                                        extComponent.Count = sText.UnicodeText;
                                    if (p.Key == "Масса 1 п.м.")
                                        extComponent.Weight = sText.UnicodeText;
                                    if (p.Key == "Примечание")
                                        extComponent.Note = sText.UnicodeText;
                                    if (p.Key == "Наименование")
                                        extComponent.Name = sText.UnicodeText;
                                }


                                if (p.Key == "Наименование" && addGost)
                                {
                                    foreach (var gost in texttable)
                                    {
                                        if (gost.StartPoint.X >= sText.StartPoint.X - 1 && gost.StartPoint.X <= sText.StartPoint.X + 1)
                                        {
                                            if (gost.StartPoint.Y >= sText.StartPoint.Y - 9 && gost.StartPoint.Y < sText.StartPoint.Y)
                                            {
                                                Console.Write($"/{gost.UnicodeText}");
                                                tempLinks.Gost = gost.UnicodeText;
                                            }
                                        }
                                    }

                                }
                            }
                        }

                    }
                }
            }
            Console.WriteLine("");
            if (tempLinks != null)
                tempLinksLst.Add(tempLinks);
            if (extComponent != null)
                extComponentLst.Add(extComponent);
        }

        //заменить текст
        public void ReplaceText(string old_str, string new_str)
        {
            Console.WriteLine(Path.GetFileName(file));
            var cadImage = CADImage.CreateImageByExtension(file);
            cadImage.LoadFromFile(file);
            for (int i = 0; i < cadImage.CurrentLayout.Count; i++)
            {
                if (cadImage.CurrentLayout.Entities[i].EntType == EntityType.Text)
                {
                    CADText vText = (CADText)cadImage.CurrentLayout.Entities[i];
                    if (vText.Text == old_str)
                    {
                        vText.Text = new_str;
                        vText.Loaded(cadImage.Converter);
                        Console.WriteLine($@"Замена произведена в файле {file}");
                        if (cadImage != null)
                        {
                            CADImport.Export.Formats.CADtoDWGFrame vExp;
                            vExp = new CADImport.Export.Formats.CADtoDWGFrame(cadImage);
                            vExp.SaveToFile("replaced" + file);
                        }
                    }
                }
            }
        }

        //добавить в ряд
        private void AddInRow(List<CADText> texttable, CADText example, List<CADText> collection)
        {
            foreach (var sText in texttable)
            {
                if (sText.StartPoint.Y >= example.StartPoint.Y - 2 && sText.StartPoint.Y <= example.StartPoint.Y + 2)
                {
                    if (sText.StartPoint.X >= example.StartPoint.X)
                    {
                        collection.Add(sText);
                        Console.WriteLine(sText.Text);
                    }
                }
            }
        }

        //парсинг коэфф жесткости если их нет во вставке
        private void ParseJJWW(List<CADText> texttable)
        {
            var valuesJ = new List<CADText>();
            var valuesW = new List<CADText>();
            string Jx = string.Empty;
            string Jy = string.Empty;
            string Wx = string.Empty;
            string Wy = string.Empty;
            foreach (var sText in texttable)
            {
                if (sText.UnicodeText.ToLower().Contains("j"))
                {
                    valuesJ.Add(sText);
                }
                if (sText.UnicodeText.ToLower().Contains("w"))
                {
                    valuesW.Add(sText);
                }
            }
            if (valuesJ.Count == 2 && valuesW.Count == 2)
            {
                foreach (var sText in valuesJ)
                {
                    if (valuesJ[0].StartPoint.X < valuesJ[1].StartPoint.X)
                    {
                        Jx = ParseJJWWColumn(texttable, valuesJ[0]);
                        Jy = ParseJJWWColumn(texttable, valuesJ[1]);
                    }
                    else
                    {
                        Jx = ParseJJWWColumn(texttable, valuesJ[1]);
                        Jy = ParseJJWWColumn(texttable, valuesJ[0]);
                    }
                }
                foreach (var sText in valuesW)
                {
                    if (valuesW[0].StartPoint.X < valuesW[1].StartPoint.X)
                    {
                        Wx = ParseJJWWColumn(texttable, valuesW[0]);
                        Wy = ParseJJWWColumn(texttable, valuesW[1]);
                    }
                    else
                    {
                        Wx = ParseJJWWColumn(texttable, valuesW[1]);
                        Wy = ParseJJWWColumn(texttable, valuesW[0]);
                    }
                }
            }
            Console.WriteLine($"J(x) - {Jx}; W(x) - {Wx}; J(y) - {Jy}; W(y) - {Wy}");
            draft.Wx = Wx;
            draft.Wy = Wy;
            draft.Jx = Jx;
            draft.Jy = Jy;
        }

        //взять значение под заголовком
        private string ParseJJWWColumn(List<CADText> texttable, CADText example)
        {
            string value = string.Empty;
            foreach (var sText in texttable)
            {
                if (sText.Point.Y >= example.Point.Y - 15 && sText.Point.Y <= example.Point.Y - 8)
                {
                    if (sText.Point.X >= example.Point.X - 8 && sText.Point.X <= example.Point.X + 8)
                    {
                        value = sText.UnicodeText;
                    }
                }
            }

            return value;
        }
    }
}
