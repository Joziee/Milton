using Milton.Website.Controllers.Recon;
using SpreadsheetGear;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Utilities
{
    public partial class UtilitiesController
    {
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadHealthShareReport()
        {
            var reconFile = this.Request.Files["HealthShareReportFile"];

            var reconData = reconFile.InputStream.GetBytes();

            string reconExtension = Path.GetExtension(reconFile.FileName);

            if ((reconFile.ContentType == "application/vnd.ms-excel" || reconFile.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") && (reconExtension == ".xlsx" || reconExtension == ".xls"))
            {
                //Use spreadsheetgear to read the file.
                //If it fails to read then notify the user that the file is invalid.
                IWorkbook reconWorkbook = null;
                try
                {
                    reconWorkbook = SpreadsheetGear.Factory.GetWorkbookSet().Workbooks.OpenFromMemory(reconData);
                }
                catch (Exception)
                {
                    throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");
                }

                if (reconWorkbook == null) throw new Exception("Unable to open file. Please upload a valid Excel file in the same format as the one provided.");

                //Validate the number of the required worksheets.
                IWorksheet reconSheet = reconWorkbook.Worksheets[0];
                if (reconSheet == null) throw new Exception("Recon Sheet is missing. Please upload an Excel file in the same format as the one provided.");

                #region Validate Data
                DataTable reconSheetData = new DataTable();

                List<FileContentException.FileContentError> reconSheetDataIssues = ValidateReconSheetData(reconSheet, out reconSheetData);

                //Join the 2 lists of issues.
                FileContentException dataIssues = new FileContentException();
                dataIssues.Errors.AddRange(reconSheetDataIssues);

                //if (dataIssues != null && dataIssues.Errors.Count > 0) throw dataIssues;
                #endregion Validate Data

                InsertData(reconSheetData);
            }
            else
            {
                throw new Exception("Invalid file type uploaded. Please upload an Excel file in the same format as the one provided.");
            }

            return RedirectToAction("Index");
        }

        /// <summary>
		/// Check that the data in the provided sheet matches the data format allowed for the import sheet 1.
		/// </summary>
		/// <param name="sheet"></param>
		/// <param name="importData"></param>
		/// <returns></returns>
		private List<FileContentException.FileContentError> ValidateReconSheetData(IWorksheet sheet, out DataTable importData)
        {
            IRange cells = sheet.Cells;
            List<FileContentException.FileContentError> issues = new List<FileContentException.FileContentError>();
            int currentRowIndex = 1;
            int emptyRowTracker = 0;
            int maxEmptyRows = 1;

            //Build the DataTable.
            importData = BuildReconSheetDataHolder();

            while (emptyRowTracker < maxEmptyRows)
            {
                //Check if the current row should be skipped.
                bool skipItem = true;

                for (int i = 0; i < importData.Columns.Count; i++)
                    if (cells[currentRowIndex, i] != null && cells[currentRowIndex, i].Value != null && !String.IsNullOrEmpty(cells[currentRowIndex, i].Value.ToString()))
                        skipItem = false;

                if (skipItem)
                {
                    currentRowIndex++;
                    emptyRowTracker++;
                    continue;
                }
                else
                {
                    emptyRowTracker = 0;
                }

                //Validate the column values.
                string invoiceDate = cells[currentRowIndex, 0].Text;
                DateTime dtInvoiceDate = DateTime.MinValue;
                if (!DateTime.TryParse(invoiceDate, out dtInvoiceDate))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Invoice Date",
                        ColumnNumber = (importData.Columns.IndexOf("Invoice Date") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Invoice Date missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string captureDate = cells[currentRowIndex, 1].Text;
                DateTime dtCaptureDate = DateTime.MinValue;
                if (!DateTime.TryParse(captureDate, out dtCaptureDate))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Capture Date",
                        ColumnNumber = (importData.Columns.IndexOf("Capture Date") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Capture Date missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string invoiceNumber = GetCellValue(cells[currentRowIndex, 2]);
                if (invoiceNumber == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Invoice Number",
                        ColumnNumber = (importData.Columns.IndexOf("InvoiceNumber") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Invoice Number missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string patient = GetCellValue(cells[currentRowIndex, 3]);
                if (patient == null)
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Patient",
                        ColumnNumber = (importData.Columns.IndexOf("Patient") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Name and Surname missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });

                string idNumber = GetCellValue(cells[currentRowIndex, 4]);
                if (idNumber == null)
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "ID Number",
                        ColumnNumber = (importData.Columns.IndexOf("IdNumber") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Amount missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string treatingPractitioner = GetCellValue(cells[currentRowIndex, 5]);
                if (treatingPractitioner == null)
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Treating Practitioner",
                        ColumnNumber = (importData.Columns.IndexOf("TreatingPractitioner") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Treating Practitioner missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string medicalAid = GetCellValue(cells[currentRowIndex, 6]);
                if (medicalAid == null)
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Medical Aid",
                        ColumnNumber = (importData.Columns.IndexOf("MedicalAid") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Medical Aid missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string code = GetCellValue(cells[currentRowIndex, 7]);
                if (code == null)
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Code",
                        ColumnNumber = (importData.Columns.IndexOf("Code") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Code missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string description = GetCellValue(cells[currentRowIndex, 8]);
                if (description == null)
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Description",
                        ColumnNumber = (importData.Columns.IndexOf("Description") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Description missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string qty = GetCellValue(cells[currentRowIndex, 9]);
                Int32 quantity = 0;
                if (!Int32.TryParse(qty, out quantity))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Qty",
                        ColumnNumber = (importData.Columns.IndexOf("Qty") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Qty missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                string total = GetCellValue(cells[currentRowIndex, 10]);
                Decimal dTotal = 0;
                if (!Decimal.TryParse(total, out dTotal))
                {
                    issues.Add(new FileContentException.FileContentError
                    {
                        ColumnName = "Total",
                        ColumnNumber = (importData.Columns.IndexOf("Total") + 1),
                        ErrorCode = 0,
                        ErrorMessage = "Total missing on Recon Sheet, Row " + (currentRowIndex + 1) + ". Please fill it in.",
                        RowNumber = (currentRowIndex + 1),
                        SheetNumber = 1
                    });
                }

                importData.Rows.Add
                    (
                        dtInvoiceDate,
                        dtCaptureDate,
                        invoiceNumber,
                        patient,
                        idNumber,
                        treatingPractitioner,
                        medicalAid,
                        code,
                        description,
                        qty,
                        total
                    );

                currentRowIndex++;
            }

            return issues;
        }

        /// <summary>
		/// Bulk insert data into SqlServer
		/// </summary>
		/// <param name="importProduct"></param>
		/// <param name="importProductVariant"></param>
		private void InsertData(DataTable importRecon)
        {
            int counter1 = 0;
            for (int i = counter1; i < importRecon.Rows.Count; i++)
            {
                DateTime invoiceDate = DateTime.Parse(importRecon.Rows[i][0].ToString());
                DateTime captureDate = DateTime.Parse(importRecon.Rows[i][1].ToString());
                string invoiceNumber = importRecon.Rows[i][2].ToString();
                string patient = importRecon.Rows[i][3].ToString();
                string idNumber = importRecon.Rows[i][4].ToString();
                string treatingPractitioner = importRecon.Rows[i][5].ToString();
                string medicalAid = importRecon.Rows[i][6].ToString();
                string code = importRecon.Rows[i][7].ToString();
                string description = importRecon.Rows[i][8].ToString();
                int qty = 0;
                if (!Int32.TryParse(importRecon.Rows[i][9].ToString(), out qty))
                {
                    var tmp = "";
                }

                if(invoiceNumber == "CON999")
                {
                    var tmp = "";
                }

                double total = Double.Parse(importRecon.Rows[i][10].ToString());

                var exist = _healthShareReconService.GetByCriteria(invoiceDate, idNumber, invoiceNumber, total);
                if (exist == null)
                {
                    var model = new Database.Models.Medical.HealthShareRecon()
                    {
                        CaptureDate = captureDate,
                        Code = code,
                        Description = description,
                        IdNumber = idNumber,
                        InvoiceDate = invoiceDate,
                        InvoiceNumber = invoiceNumber,
                        MedicalAid = medicalAid,
                        Patient = patient,
                        Qty = qty,
                        Total = total,
                        TreatingPractitioner = treatingPractitioner
                    };

                    _healthShareReconService.Insert(model);
                }
            }
        }

        /// <summary>
		/// Build a DataTable that can be used for the 1st sheet.
		/// </summary>
		/// <returns></returns>
		private DataTable BuildReconSheetDataHolder()
        {
            DataTable table = new DataTable();

            table.Columns.Add("InvoiceDate");
            table.Columns.Add("CaptureDate");
            table.Columns.Add("InvoiceNumber");
            table.Columns.Add("Patient");
            table.Columns.Add("IdNumber");
            table.Columns.Add("TreatingPractitioner");
            table.Columns.Add("MedicalAid");
            table.Columns.Add("Code");
            table.Columns.Add("Description");
            table.Columns.Add("Qty");
            table.Columns.Add("Total");

            return table;
        }

        /// <summary>
		/// Validate a cell and return its value if any.
		/// </summary>
		/// <param name="cell"></param>
		/// <returns></returns>
		private string GetCellValue(IRange cell)
        {
            return cell != null
                    && cell.Value != null
                    && !String.IsNullOrEmpty(cell.Value.ToString().Trim())
                    && !cell.Value.ToString().ToLower().Trim().Equals("n/a")
                    && !cell.Value.ToString().ToLower().Trim().Equals("n\a")
                    && !cell.Value.ToString().ToLower().Trim().Equals("n.a") ? cell.Value.ToString().Trim().Replace("'", "''") : null;
        }
    }
}
