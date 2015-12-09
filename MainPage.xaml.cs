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
using PrintSample.Resources;
using Windows.Networking.Proximity;
using HSM.Mobility.Printing;
using Windows.Networking.Sockets;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Resources;

namespace PrintSample
{
   
    public partial class MainPage : PhoneApplicationPage
    {
        private LinePrinter _objLinePrinter = null; // Line printer object
        private LabelPrinter _objLabelPrinter = null; // Label printer object
        private bool _bIsConnected = false; // printer connection check
        private bool _bIsLinePrinter = false; // Line printer check

        private string _strMacAddr = "";      // mac addr
        private string _strPrinterName = "";   // printer name
        private const int MACADDRLEN = 12;
        private ResourceManager _rm = null;

        // progress event
        public delegate void ProgressCallbackHandler(UInt64 handle, Int32 objLinePrinteraram);
        public event ProgressCallbackHandler ProgressCallbackHandlerEx = null;

        // error event
        public delegate void ErrorCallbackHandler(UInt64 handle, Int32 objLinePrinteraram, string msg);
        public event ErrorCallbackHandler ErrorCallbackHandlerEx = null;

        // constructor
        /// ************************************************************************************************
        /// <summary>
        /// MainPage 
        /// </summary>
        /// <remarks>
        /// constructor
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public MainPage()
        {
            InitializeComponent();
            EnableControls(false);
            // PB32
            _strMacAddr = this.txtMacAddr.Text = "00A0961193DC"; //"00066602C42A";
            _strPrinterName = this.txtPrinterName.Text = "PB42"; // "PB32_Fingerprint";
            // PR2
            //_strMacAddr = this.txtMacAddr.Text = "001DDF58DD93";  //-PR2  
            //_strPrinterName = this.txtPrinterName.Text = "PR2";
            this.rbLinePrint.IsChecked = true;
            _rm = new ResourceManager("PrintSample.StringLibrary", Assembly.GetExecutingAssembly());
            // status message
            var prog = new ProgressIndicator();
            this.txtMacAddr.MaxLength = MACADDRLEN;
            if(null != _rm)
                prog.Text = _rm.GetString("BT_CONNECT");
            prog.IsVisible = true;
            prog.IsIndeterminate = false;
            SystemTray.SetProgressIndicator(this, prog);               
        }
        /// ************************************************************************************************
        /// <summary>
        /// EnableControls
        /// </summary>
        /// <remarks>
        /// Enables / Disables UI Control
        /// <param name="bEnable">Enable</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableControls(bool bEnable)
        {
            _bIsConnected = bEnable;
            this.btnPrintLabel.IsEnabled = bEnable;
            this.btnDisconnect.IsEnabled = bEnable;
            this.btnPrintReciept.IsEnabled = bEnable;
            ChangerbOptions(bEnable);
        }
        /// ************************************************************************************************
        /// <summary>
        /// EnableLabelPrintingControls
        /// </summary>
        /// <remarks>
        /// Enables LabelPrinting Controls
        /// <param name="bEnable">Enable</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableLabelPrintingControls(bool bEnable)
        {
            _bIsConnected = bEnable;           
            this.btnPrintLabel.IsEnabled = bEnable;
            this.btnDisconnect.IsEnabled = bEnable;
            this.btnPrintReciept.IsEnabled = false;
            ChangerbOptions(bEnable);
        }
        /// ************************************************************************************************
        /// <summary>
        /// EnableLinePrintingControls
        /// </summary>
        /// <remarks>
        /// Enables LinePrinting Controls
        /// <param name="bEnable">bEnable</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableLinePrintingControls(bool bEnable)
        {
            _bIsConnected = bEnable;
            this.btnPrintLabel.IsEnabled = false;
            this.btnDisconnect.IsEnabled = bEnable;
            this.btnPrintReciept.IsEnabled = bEnable;
            ChangerbOptions(bEnable);
        }
        /// ************************************************************************************************
        /// <summary>
        /// ChangerbOptions
        /// </summary>
        /// <remarks>
        /// Enables radio button options
        /// <param name="bEnable">bEnable</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
         private void ChangerbOptions(bool bEnable)
         {
             // if connected , donot allow to select line /label  printing
             if (bEnable)
             {
                 this.rbLabelPrint.IsEnabled = false;
                 this.rbLinePrint.IsEnabled = false;
             }
             else
             {
                 this.rbLabelPrint.IsEnabled = true;
                 this.rbLinePrint.IsEnabled = true;
             }
         }
        /// ************************************************************************************************
        /// <summary>
        /// ConnectButton_Click
        /// </summary>
        /// <remarks>
        /// ConnectButton_Click event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            App.AssignMainObject(this);
            if(null !=  _rm)
                SetStatusMsg(true,_rm.GetString("CONNECT_STATUS"));
            ProgressCallbackHandlerEx = new ProgressCallbackHandler(progressStatus);
            ErrorCallbackHandlerEx = new ErrorCallbackHandler(ErrorStatus);
            CreatePrinterObject();
            if (null != _rm)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(_rm.GetString("BT_SETTINGS")));
            }
            ConnectToPrinter();           
        }
        /// ************************************************************************************************
        /// <summary>
        /// ConnectToPrinter
        /// </summary>
        /// <remarks>
        /// connects to Line printer or Label printer
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void ConnectToPrinter()
        {
            try
            {
                if (null != _objLinePrinter)
                {
                    _objLinePrinter.Connect();                    
                }
                else if (null != _objLabelPrinter)
                {
                    _objLabelPrinter.Connect();
                }
            }
            catch (Exception ex)
            {
                 SetStatusMsg(false, ex.Message);               
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// UpdateStatusUI
        /// </summary>
        /// <remarks>
        /// Update Status UI message
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public bool UpdateStatusUI(Int32 progStatus)
        {
            string currentPage = App.RootFrame.Content.ToString();
            if (null == _rm)
                return false;
           if (currentPage.Contains(_rm.GetString("LBL_TITLE")) && App.objPrintLbl != null)
            {
                App.objPrintLbl.setStatus(progStatus);
                return true;
            }
           else if (currentPage.Contains(_rm.GetString("RECPT_TITLE")) && App.objPrintRecpt != null)
            {
                App.objPrintRecpt.setStatus(progStatus);
                return true;
            }
           return false;
        }
        /// ************************************************************************************************
        /// <summary>
        /// progressStatus
        /// </summary>
        /// <remarks>
        /// event - updates progress staut of printer
        /// <param name="handle">handle</param>
        /// <param name="progStatus">status value</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public void progressStatus(UInt64 handle, Int32 progStatus)
        {
            if (!UpdateStatusUI(progStatus))
            {
                switch (progStatus)
                {
                    case (int)PrinterProgress.MSG_CANCEL:
                        SetStatusMsg(false, App.PROGRESS_CANCEL_MSG);
                        break;
                    case (int)PrinterProgress.MSG_COMPLETE:
                        SetStatusMsg(true, App.PROGRESS_COMPLETE_MSG);
                        break;
                    case (int)PrinterProgress.MSG_ENDDOC:
                        SetStatusMsg(true, App.PROGRESS_ENDDOC_MSG);
                        break;
                    case (int)PrinterProgress.MSG_FINISHED:
                        SetStatusMsg(true, App.PROGRESS_FINISHED_MSG);
                        break;
                    case (int)PrinterProgress.MSG_STARTDOC:
                        {
                            if (_bIsLinePrinter)
                                EnableLinePrintingControls(true);
                            else
                                EnableLabelPrintingControls(true);
                            this.btnConnect.IsEnabled = false;
                            this.btnDisconnect.IsEnabled = true;
                            if(null != _rm)
                                 SetStatusMsg(true, _rm.GetString("CONNECT_SUCCESS"));

                        }
                        break;
                    default:
                        SetStatusMsg(false, App.PROGRESS_NONE_MSG);
                        break;
                }
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// ErrorStatus
        /// </summary>
        /// <remarks>
        ///  event - updates Error status of printing process.
        /// <param name="handle">handle</param>
        /// <param name="Errcode">error code</param>
        /// <param name="msg">error msg</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public void ErrorStatus(UInt64 handle, Int32 Errcode, string msg)
        {
            try
            {
                if (!UpdateStatusUI(Errcode))
                {
                    if (Errcode == 0)
                    {
                        if (_bIsLinePrinter)
                            EnableLinePrintingControls(true);
                        else
                            EnableLabelPrintingControls(true);
                        if(_rm != null)
                            SetStatusMsg(true, _rm.GetString("CONNECT_SUCCESS"));
                    }
                    else if (Errcode <= 6)
                    {
                        //connect failed                       
                        SetStatusMsg(false, msg);
                        EnableControls(false);
                        this.btnConnect.IsEnabled = true;
                        this.btnDisconnect.IsEnabled = false;
                    }                 
                }

            }
            catch (Exception ex)
            {
                SetStatusMsg(false, ex.Message);
                //  throw;
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// CreatePrinterObject
        /// </summary>
        /// <remarks>
        /// creates Line printer or label printer object
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void CreatePrinterObject()
        {
            try
            {
                if (_strPrinterName != "")
                {
                    int nRet = -1;
                    if (null == _rm)
                        return;
                    if (_strPrinterName.ToLower().Contains(_rm.GetString("FP")) == true && !_bIsLinePrinter)
                    {
                        _objLabelPrinter = new LabelPrinter(Constants.JSON_FILE, _strPrinterName, _strMacAddr);
                        nRet = _objLabelPrinter.GetPrintHandle();
                        if( nRet == 0)
                        { 
                        _objLabelPrinter.RegisterProgressEvent(progressStatus);
                        _objLabelPrinter.RegisterErrorEvent(ErrorStatus);
                        App.LabelPrinterObject = _objLabelPrinter;
                        }
                    }
                    else
                    {
                        _objLinePrinter = new LinePrinter(Constants.JSON_FILE, _strPrinterName, _strMacAddr);
                        nRet = _objLinePrinter.GetPrintHandle();
                        if (nRet == 0)
                        {
                        _objLinePrinter.RegisterProgressEvent(progressStatus);
                        _objLinePrinter.RegisterErrorEvent(ErrorStatus);
                        App.LinePrinterObject = _objLinePrinter;
                        }
                        else
                        {
                            switch(nRet)
                            {
                                case (int)PrinterError.ERROR_FILE_NOTFOUND:
                                    if (null != _rm)
                                        SetStatusMsg(false, _rm.GetString("ERR_JSON_FILE"));
                                    break;
                                case (int)PrinterError.ERROR_INVALID_MACADDRESS:
                                    if (null != _rm)
                                        SetStatusMsg(false, _rm.GetString("ERR_MAC_ADDR"));
                                    break;
                                case (int)PrinterError.ERROR_INVALID_PRINTER_ID:
                                    if (null != _rm)
                                        SetStatusMsg(false, _rm.GetString("ERR_PRNTR_ID"));
                                    break;
                                default:
                                    if (null != _rm)
                                        SetStatusMsg(false, _rm.GetString("ERR_INVALID_PARAM"));
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception )
            {
                if (null != _rm)
                    SetStatusMsg(false,  _rm.GetString("ERR_INVALID_PARAM"));
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// SetStatusMsg
        /// </summary>
        /// <remarks>
        /// updates status msg
        /// <param name="bError">error</param>
        /// <param name="strMsg">msg</param>
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
       private void SetStatusMsg(bool bError,string strMsg)
        {
            SystemTray.IsVisible = true;
            var prog = new ProgressIndicator();
            prog.Text = strMsg;  
            prog.IsVisible = true;
            prog.IsIndeterminate = false;
            if (bError)
             SystemTray.SetBackgroundColor(this,System.Windows.Media.Color.FromArgb(0,0,255,0)); // green
           else
             SystemTray.SetBackgroundColor(this,System.Windows.Media.Color.FromArgb(0,255,0,0)); // red
            SystemTray.SetProgressIndicator(this, prog);
        }
       /// ************************************************************************************************
       /// <summary>
       /// PrintBarcodeButton_Click
       /// </summary>
       /// <remarks>
       /// PrintBarcodeButton_Click event
       /// <Development> Implemented. </Development>                                         
       /// ************************************************************************************************
        private void PrintBarcodeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.Navigate(new Uri("/PrintBarcode.xaml", UriKind.Relative));              
            }
            catch (Exception)
            {
                 SetStatusMsg(false, "Error in Printing barcode,please check connection");             
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// DisconnectButton_Click
        /// </summary>
        /// <remarks>
        /// Disconnect printer
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SystemTray.IsVisible = true;
                if (null != _objLinePrinter)
                {
                    _objLinePrinter.Flush();                    
                    if (_objLinePrinter.Disconnect() == 0)
                    {
                        if(null != _rm)
                             SetStatusMsg(true,  _rm.GetString("DISCONNECT_SUCCESS"));
                        this.btnConnect.IsEnabled = true;
                        this.btnDisconnect.IsEnabled = false;
                        EnableControls(false);                 
                    }
                }
                else if(null != _objLabelPrinter)
                {
                    if (_objLabelPrinter.Disconnect() == 0)
                    {
                        if (null != _rm)
                            SetStatusMsg(true,  _rm.GetString("DISCONNECT_SUCCESS"));
                        this.btnConnect.IsEnabled = true;
                        this.btnDisconnect.IsEnabled = false;
                        EnableControls(false);
                    }
                }
            }
            catch (Exception )
            {
               if(null != _rm)
                 SetStatusMsg(false, _rm.GetString("INVALID_ERR"));               
            }

        }
        /// ************************************************************************************************
        /// <summary>
        /// txtInput_TextChanged
        /// </summary>
        /// <remarks>
        /// Mac address input
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            _strMacAddr = this.txtMacAddr.Text;          
        }
        /// ************************************************************************************************
        /// <summary>
        /// PrinterRecieptButton_Click
        /// </summary>
        /// <remarks>
        /// navigates to Print reciept UI
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void PrinterRecieptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (null != _rm)
                     NavigationService.Navigate(new Uri("/" + _rm.GetString("RECPT_TITLE") + ".xaml", UriKind.Relative));
                SetStatusMsg(true, "");
            }
            catch (Exception)
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));  
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtPrinterName_TextChanged
        /// </summary>
        /// <remarks>
        /// printer name input
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtPrinterName_TextChanged(object sender, TextChangedEventArgs e)
        {
             _strPrinterName = this.txtPrinterName.Text;
        }
        /// ************************************************************************************************
        /// <summary>
        /// PrintLabelButton_Click
        /// </summary>
        /// <remarks>
        /// navigates to Print Label screen.
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void PrintLabelButton_Click(object sender, RoutedEventArgs e)
        {
            if (null != _rm)
                NavigationService.Navigate(new Uri("/"+ _rm.GetString("LBL_TITLE")+ ".xaml", UriKind.Relative));           
            SetStatusMsg(true, "");
        }
        /// ************************************************************************************************
        /// <summary>
        /// rbLinePrint_Checked
        /// </summary>
        /// <remarks>
        /// rbLinePrint_Checked EVENT
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void rbLinePrint_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbLinePrint.IsChecked == true)
                  _bIsLinePrinter = true;
            else
                 _bIsLinePrinter = false;
        }
        /// ************************************************************************************************
        /// <summary>
        /// rbLabelPrint_Checked
        /// </summary>
        /// <remarks>
        /// rbLabelPrint_Checked EVENT
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void rbLabelPrint_Checked(object sender, RoutedEventArgs e)
        {
            if (this.rbLabelPrint.IsChecked == true)
            {
                if (null == _rm)
                    return;
                if (_strPrinterName != "" && !_strPrinterName.ToLower().Contains(_rm.GetString("FP")))
                {                    
                    MessageBox.Show(_strPrinterName +  _rm.GetString("ERR_LBL_PRINT"));
                    this.rbLinePrint.IsChecked = true;
                }
                else 
                   _bIsLinePrinter = false;
            }
            else
                _bIsLinePrinter = true;
        }
        /// ************************************************************************************************
        /// <summary>
        /// About_Loaded
        /// </summary>
        /// <remarks>
        /// About_Loaded Event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void About_Loaded(object sender, RoutedEventArgs e)
        {
            var pi = (sender as PivotItem);
            PrintSample.About about = new PrintSample.About();
            pi.Content = about;
            var assembly = this.GetType().GetTypeInfo().Assembly;
            string AppVer = assembly.GetName().Version.ToString();
            about.Init(assembly.GetName().Name, AppVer);
        }
     


    }
}