﻿using App_Depuraciones.Model;
using App_Depuraciones.Negocio;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using App_Depuraciones.Base;
using EntityFramework.BulkInsert.Extensions;

namespace App_Depuraciones
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        private void ConfirmacionMSBGSi()
        {
            var url = "C:\\Archivos\\ObservacionesMSBG.xlsx";
            IEnumerable<Formatokia> _model = null;
            using (FileStream fileStream = new FileStream(url.ToString(), FileMode.Open))
            {
                using (ExcelPackage package = new ExcelPackage(fileStream))
                {
                    ExcelWorkbook workbook = package.Workbook;
                    if (workbook != null)
                    {
                        ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                        if (worksheet != null)
                        {


                            _model = worksheet.ImportExcelToList<Formatokia>();
                            this.pbAvance.Maximum = _model.Count();
                            this.pbAvance.Value = 0;
                            this.lblprogreso.Text = "Progreso " + this.pbAvance.Value.ToString() + "/" + this.pbAvance.Maximum.ToString();
                            foreach (Formatokia item in _model)
                            {

                                using (var context = new dyvenproDBSEntities())
                                {
                                    context.Database.ExecuteSqlCommand($@"EXEC dbo.sp_actualiza_observacion_MSBG @dtask = '{item.idtask}', @formulario = '{item.Formulario}',@vehiculo = '{item.VehiculoCotizado}',@asesor = '{item.NombreAsesor}',@observacion = '{item.Observacion}'");
                                    this.pbAvance.Value++;
                                    this.lblprogreso.Text = "Progreso " + this.pbAvance.Value.ToString() + "/" + this.pbAvance.Maximum.ToString();
                                }
                            }
                        }
                    }
                }
            }
            string message = "Los datos se actualizaron correctamente";
            DialogResult res = MessageBox.Show(message, "Mansaje Informativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (res == DialogResult.OK)
            {
                this.pbAvance.Maximum = 0;
                this.pbAvance.Value = 0;
                this.lblprogreso.Text = "Progreso " + this.pbAvance.Value.ToString() + "/" + this.pbAvance.Maximum.ToString();
            }
        }

        private void btnMSBG_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("¿Esta Seguro que desea actualizar la información de MS Banco Guayaquil?", "Mansaje Confimación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (res == DialogResult.Yes)
            {
                ConfirmacionMSBGSi();
            }
            if (res == DialogResult.No)
            {
                this.pbAvance.Value = 0;
            }
        }
    }
}
