/**********************************************************************************************

COPYRIGHT (c) 2015
HONEYWELL INC.,
ALL RIGHTS RESERVED

This software is a copyrighted work and/or information protected 
as a trade secret. Legal rights of Honeywell Inc. in this software
is distinct from ownership of any medium in which the software is 
embodied. Copyright or trade secret notices included must be
reproduced in any copies authorized by Honeywell Inc.

The information in this software is subject to change without notice 
and should not be considered as a commitment by Honeywell Inc.

**********************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HSM.Mobility.Printing;
using System.Reflection;
using System.Resources;

namespace PrintSample
{
    public partial class PrintReciept : PhoneApplicationPage
    {
        private string _strOptional =Constants.EMPTYSTRING;
        private ResourceManager _rm = null;

        public PrintReciept()
        {
            InitializeComponent();
            this.btnPrintReciept.IsEnabled = false;          
            App.AssignRecptObject(this);
            _rm = new ResourceManager("PrintSample.StringLibrary", Assembly.GetExecutingAssembly());
        }
       
        /// ************************************************************************************************
        /// <summary>
        /// btnPrintReciept_Click 
        /// </summary>
        /// <remarks>
        /// btnPrintReciept_Click event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void btnPrintReciept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetStatusMsg(true, _rm.GetString("RECPT_PRINT_STATUS"));
                GenerateReciept();   
            }
            catch (Exception)
            {
                SetStatusMsg(false, _rm.GetString("RECPT_ERROR_STATUS"));
            }           
        }
        /// ************************************************************************************************
        /// <summary>
        /// SetStatusMsg 
        /// </summary>
        /// <remarks>
        /// updates status msg
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void SetStatusMsg(bool value, string strValue)
        {
            try
            {
                SystemTray.IsVisible = false;
                var prog = new ProgressIndicator();
                prog.Text = strValue;
                prog.IsVisible = true;
                prog.IsIndeterminate = false;
                if (value)
                    SystemTray.SetBackgroundColor(this, System.Windows.Media.Color.FromArgb(0, 0, 255, 0));
                else
                    SystemTray.SetBackgroundColor(this, System.Windows.Media.Color.FromArgb(0, 255, 0, 0));
                SystemTray.SetProgressIndicator(this, prog);
                SystemTray.IsVisible = true;
            }
            finally
            {
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// GenerateReciept 
        /// </summary>
        /// <remarks>
        /// Generate dummy Reciept 
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private bool GenerateReciept()
        {
            try
            {
                string sDocNumber = "Doc1";
                App.LinePrinterObject.NewLine(1);

                // Set font style to Bold + Double Wide + Double High.
                App.LinePrinterObject.SetBold(true);
                App.LinePrinterObject.SetDoubleWide(true);
                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("SALES ORDER");
                App.LinePrinterObject.SetDoubleWide(false);
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(2);

                // The following text shall be printed in Bold font style.
                App.LinePrinterObject.Write("CUSTOMER: Casual Step");
                App.LinePrinterObject.SetBold(false);  // Returns to normal font.
                App.LinePrinterObject.NewLine(2);

                // Set font style to Compressed + Double High.
                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.SetCompress(true);
                App.LinePrinterObject.Write("DOCUMENT#: " + sDocNumber);
                App.LinePrinterObject.SetCompress(false);
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(2);

                // The following text shall be printed in Normal font style.
                App.LinePrinterObject.Write(" PRD. DESCRIPT.   PRC.  QTY.    NET.");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write(" 1501 Timer-Md1  13.15     1   13.15");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1502 Timer-Md2  13.15     3   39.45");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1503 Timer-Md3  13.15     2   26.30");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1504 Timer-Md4  13.15     4   52.60");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write(" 1505 Timer-Md5  13.15     5   65.75");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("                        ----  ------");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("              SUBTOTAL    15  197.25");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("          5% State Tax          9.86");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.Write("                              ------");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.Write("           BALANCE DUE        207.11");
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.NewLine(1);

                App.LinePrinterObject.Write(" PAYMENT TYPE: CASH");
                App.LinePrinterObject.NewLine(2);

                App.LinePrinterObject.SetDoubleHigh(true);
                App.LinePrinterObject.Write("       SIGNATURE / STORE STAMP");
                App.LinePrinterObject.SetDoubleHigh(false);
                App.LinePrinterObject.NewLine(2);
                App.LinePrinterObject.NewLine(1);
                App.LinePrinterObject.SetBold(true);
                if (_strOptional != "")
                {
                    // Print the text entered by user in the Optional Text field.
                    App.LinePrinterObject.Write(_strOptional);
                    App.LinePrinterObject.NewLine(2);
                }
                App.LinePrinterObject.Write("          ORIGINAL");
                App.LinePrinterObject.SetBold(false);
                App.LinePrinterObject.NewLine(2);

                // Print a Code 39 barcode containing the document number.
                App.LinePrinterObject.WriteBarcode(BarcodeType.BC_CODE39,
                        sDocNumber,   // Document# to encode in barcode
                        90,           // Desired height of the barcode in printhead dots
                        40);          // Offset in printhead dots from the left of the page

                App.LinePrinterObject.NewLine(4);
                return true;
            }
            catch (Exception)
            {
                SetStatusMsg(false, _rm.GetString("RECPT_ERROR_STATUS"));
            }
            return false;
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtOptional_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtOptional_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtOptional_TextChanged(object sender, TextChangedEventArgs e)
        {
            _strOptional = this.txtOptional.Text;
            if (_strOptional != Constants.EMPTYSTRING)
            this.btnPrintReciept.IsEnabled = true;
            else
                this.btnPrintReciept.IsEnabled = false;
        }
        /// ************************************************************************************************
        /// <summary>
        /// BackButton_Click 
        /// </summary>
        /// <remarks>
        /// BackButton_Click event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void txtOptional_GotFocus(object sender, RoutedEventArgs e)
        {
            this.txtOptional.Text = "";
        }
        /// ************************************************************************************************
        /// <summary>
        /// setStatus
        /// </summary>
        /// <remarks>
        /// setStatus callback event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public void setStatus(Int32 Errcode)
        {
            if (Errcode == (int)PrinterProgress.MSG_COMPLETE)
            {
                if(null != _rm)
                     SetStatusMsg(true, _rm.GetString("RECPT_PRINT_SUCCESS"));
            }
            else
            {
                if (null != _rm)
                     SetStatusMsg(false, _rm.GetString("RECPT_ERROR_STATUS"));
            }
        }
    }
}