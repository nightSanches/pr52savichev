using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using pr52savichev.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace pr52savichev.Classes.Common
{
    public class Report
    {
        public static void Group(int IdGroup, Main main)
        {
            SaveFileDialog SFD = new SaveFileDialog
            {
                InitialDirectory = @"C:\",
                Filter = "Excel (*.xlsx)|*.xlsx"
            };
            SFD.ShowDialog();

            if (SFD.FileName != "")
            {
                GroupContext Group = main.AllGroups.Find(x => x.Id == IdGroup);
                var ExcelApp = new Excel.Application();

                try
                {
                    ExcelApp.Visible = false;
                    Excel.Workbook Workbook = ExcelApp.Workbooks.Add(Type.Missing);
                    Excel.Worksheet Worksheet = Workbook.ActiveSheet;

                    (Worksheet.Cells[1, 1] as Excel.Range).Value = $"Отчет о группе {Group.Name}";
                    Worksheet.Range[Worksheet.Cells[1, 1], Worksheet.Cells[1, 5]].Merge();
                    Styles(Worksheet.Cells[1, 1], 18);

                    (Worksheet.Cells[3, 1] as Excel.Range).Value = $"Список группы: ";
                    Worksheet.Range[Worksheet.Cells[3, 1], Worksheet.Cells[3, 5]].Merge();
                    Styles(Worksheet.Cells[3, 1], 12, Excel.XlHAlign.xlHAlignLeft);

                    (Worksheet.Cells[4, 1] as Excel.Range).Value = $"ФИО";
                    Styles(Worksheet.Cells[4, 1], 12, Excel.XlHAlign.xlHAlignCenter, true);
                    (Worksheet.Cells[4, 1] as Excel.Range).ColumnWidth = 35.0f;

                    (Worksheet.Cells[4, 2] as Excel.Range).Value = $"Кол-во не сданных практических: ";
                    Styles(Worksheet.Cells[4, 2], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    (Worksheet.Cells[4, 3] as Excel.Range).Value = $"Кол-во не сданных теоретических: ";
                    Styles(Worksheet.Cells[4, 3], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    (Worksheet.Cells[4, 4] as Excel.Range).Value = $"Отсутствовал на паре: ";
                    Styles(Worksheet.Cells[4, 4], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    (Worksheet.Cells[4, 5] as Excel.Range).Value = $"Опоздал: ";
                    Styles(Worksheet.Cells[4, 5], 12, Excel.XlHAlign.xlHAlignCenter, true);

                    int Height = 5;
                    int bestStudent = 1000;
                    int currBestStudent = 0;
                    int idBestStudent = 5;
                    List<StudentContext> Students = main.AllStudents.FindAll(x => x.IdGroup == IdGroup);


                    foreach (StudentContext Student in Students)
                    {
                        List<DisciplineContext> StudentDisciplines = main.AllDisciplines.FindAll(
                            x => x.IdGroup == Student.IdGroup);
                        int PracticeCount = 0;
                        int TheoryCount = 0;
                        int AbsenteeismCount = 0;
                        int LateCount = 0;

                        foreach (DisciplineContext StudentDiscipline in StudentDisciplines)
                        {
                            List<WorkContext> StudentsWorks = main.AllWorks.FindAll(x => x.IdDiscipline == StudentDiscipline.Id);

                            foreach (WorkContext StudentWork in StudentsWorks)
                            {
                                EvaluationContext Evaluation = main.AllEvaluation.Find(x =>
                                    x.IdWork == StudentWork.Id &&
                                    x.IdStudent == Student.Id);

                                if ((Evaluation != null && (Evaluation.Value.Trim() == "" || Evaluation.Value.Trim() == "2")) || Evaluation == null)
                                {
                                    if (StudentWork.IdType == 1)
                                        PracticeCount++;
                                    else if (StudentWork.IdType == 2)
                                        TheoryCount++;
                                }
                                if (Evaluation != null && Evaluation.Lateness.Trim() != "")
                                {
                                    if (Convert.ToInt32(Evaluation.Lateness) == 90)
                                        AbsenteeismCount++;
                                    else
                                        LateCount++;
                                }
                            }
                        }

                        currBestStudent = PracticeCount + TheoryCount + AbsenteeismCount + LateCount;
                        if (currBestStudent < bestStudent)
                        {
                            bestStudent = currBestStudent;
                            idBestStudent = Height;
                        }

                        (Worksheet.Cells[Height, 1] as Excel.Range).Value = $"{Student.Lastname} {Student.Firstname}";
                        Styles(Worksheet.Cells[Height, 1], 12, XlHAlign.xlHAlignLeft, true);

                        (Worksheet.Cells[Height, 2] as Excel.Range).Value = PracticeCount.ToString();
                        Styles(Worksheet.Cells[Height, 2], 12, XlHAlign.xlHAlignCenter, true);

                        (Worksheet.Cells[Height, 3] as Excel.Range).Value = TheoryCount.ToString();
                        Styles(Worksheet.Cells[Height, 3], 12, XlHAlign.xlHAlignCenter, true);

                        (Worksheet.Cells[Height, 4] as Excel.Range).Value = AbsenteeismCount.ToString();
                        Styles(Worksheet.Cells[Height, 4], 12, XlHAlign.xlHAlignCenter, true);

                        (Worksheet.Cells[Height, 5] as Excel.Range).Value = LateCount.ToString();
                        Styles(Worksheet.Cells[Height, 5], 12, XlHAlign.xlHAlignCenter, true);


                        Height++;
                    }


                    foreach (StudentContext Student in Students)
                    {
                        Worksheet worksheet = Workbook.Sheets.Add(After: Workbook.Sheets[Workbook.Sheets.Count]);
                        worksheet.Name = $"{Student.Firstname} {Student.Lastname}";
                        Excel.Range range = worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 2]];
                        (worksheet.Cells[1, 1] as Excel.Range).Value = $"{Student.Lastname}   {Student.Firstname}";
                        range.Merge();
                        range.ColumnWidth = 30;
                        (worksheet.Cells[3, 1] as Excel.Range).Value = $"Не сданные работы";
                        (worksheet.Cells[3, 2] as Excel.Range).Value = $"Сданные работы";

                        List<EvaluationContext> submittedWork = main.AllEvaluation.FindAll(x => x.IdStudent == Student.Id && !String.IsNullOrEmpty(x.Value));
                        List<EvaluationContext> notSubmittedWork = main.AllEvaluation.FindAll(x => x.IdStudent == Student.Id && String.IsNullOrEmpty(x.Value));


                        int number = 4;
                        foreach (EvaluationContext evaluation in submittedWork)
                        {
                            (worksheet.Cells[number, 2] as Excel.Range).Value = main.AllWorks.Find(x => x.Id == evaluation.IdWork).Name;

                            number++;
                        }
                        int number2 = 4;
                        foreach (EvaluationContext workContext in notSubmittedWork)
                        {
                            (worksheet.Cells[number2, 1] as Excel.Range).Value = main.AllWorks.Find(x => x.Id == workContext.IdWork).Name;
                            number2++;
                        }
                    }
                    Worksheet.Activate();
                    Workbook.SaveAs(SFD.FileName);
                    Workbook.Close();
                    ExcelApp.Quit();
                }
                catch (Exception exp) { MessageBox.Show(exp.Message); };


            }
        }

        public static void students(Excel.Workbook workbook, string student, Main main, List<StudentContext> Students)
        {
            foreach (StudentContext Student in Students)
            {
                Worksheet Worksheet = workbook.Sheets.Add(After: workbook.Sheets[workbook.Sheets.Count]);
                Worksheet.Name = $"{student}";
                Excel.Range range = Worksheet.Range[Worksheet.Cells[1, 1], Worksheet.Cells[1, 2]];
                (Worksheet.Cells[1, 1] as Excel.Range).Value = $"{student}";
                range.Merge();
                range.ColumnWidth = 30;
                (Worksheet.Cells[3, 1] as Excel.Range).Value = $"Не сданные работы";
                (Worksheet.Cells[3, 2] as Excel.Range).Value = $"Сданные работы";

                List<EvaluationContext> evaluations = main.AllEvaluation.FindAll(x => x.IdStudent == Student.Id);
            }

        }

        public static void Styles(Excel.Range Cell, int FontSize, Excel.XlHAlign Position = Excel.XlHAlign.xlHAlignCenter,
                                    bool Border = false)
        {
            Cell.Font.Name = "Bahnschrift Light Condensed";
            Cell.Font.Size = FontSize;

            Cell.HorizontalAlignment = Position;
            Cell.VerticalAlignment = Excel.XlHAlign.xlHAlignCenter;

            if (Border)
            {
                Excel.Borders border = Cell.Borders;
                border.LineStyle = Excel.XlLineStyle.xlDouble;
                border.Weight = XlBorderWeight.xlThin;
                Cell.WrapText = true;
            }
        }
    }
}
