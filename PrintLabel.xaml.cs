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
    public partial class PrintLabel : PhoneApplicationPage
    {
        private string _strItemName = Constants.EMPTYSTRING;  // Item Label 
        private string _strItemNo = Constants.EMPTYSTRING;    // Iteem no
        private string _strURL = Constants.EMPTYSTRING;       // URL label
        private string _strTextLine1 = Constants.EMPTYSTRING; // TextLine1
        private string _strTextLine2 = Constants.EMPTYSTRING; // TextLine2
        private bool _bIsItemLabel = Constants.ISITEMLBL; // Is Item Label
        private ResourceManager _rm = null;

        /// ************************************************************************************************
        /// <summary>
        /// PrintLabel 
        /// </summary>
        /// <remarks>
        /// constructor
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        public PrintLabel()
        {
            InitializeComponent();
            this.btnPrintLabel.IsEnabled = false;
            EnableItemControls(false);
            EnableURLControls(false);
            App.AssignLabelObject(this);
            _rm = new ResourceManager("PrintSample.StringLibrary", Assembly.GetExecutingAssembly());

        }
        /// ************************************************************************************************
        /// <summary>
        /// EnableItemControls 
        /// </summary>
        /// <remarks>
        /// EnableItem label related Controls 
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableItemControls(bool bEnable)
        {
            _bIsItemLabel = bEnable;
            this.txtItemName.IsEnabled = bEnable;
            this.txtItemNo.IsEnabled = bEnable;
            UpdatePrintLabelBtn();
        }
        /// ************************************************************************************************
        /// <summary>
        /// UpdatePrintLabelBtn 
        /// </summary>
        /// <remarks>
        /// Enable PrintLabel button based on user inputs 
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void UpdatePrintLabelBtn()
        {
            if (_bIsItemLabel)
            {
                if (_strItemNo != Constants.EMPTYSTRING &&
                    _strItemName != Constants.EMPTYSTRING)
                    this.btnPrintLabel.IsEnabled = true;
                else
                    this.btnPrintLabel.IsEnabled = false;
            }
            else
            {
                if (_strURL != Constants.EMPTYSTRING && 
                    _strTextLine1 != Constants.EMPTYSTRING && 
                    _strTextLine2 != Constants.EMPTYSTRING)
                    this.btnPrintLabel.IsEnabled = true;
                else
                    this.btnPrintLabel.IsEnabled = false;
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// EnableURLControls 
        /// </summary>
        /// <remarks>
        /// Enables URL Label related Controls
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void EnableURLControls(bool bEnable)
        {
            _bIsItemLabel = false;
            this.txtURL.IsEnabled = bEnable;
            this.txtTextLine1.IsEnabled = bEnable;
            this.txtTextLine2.IsEnabled = bEnable;
            UpdatePrintLabelBtn();
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtItemName_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtItemName_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strItemName = this.txtItemName.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// btnPrintLabel_Click 
        /// </summary>
        /// <remarks>
        /// btnPrintLabel_Click event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void btnPrintLabel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int ret = -1;
               if (_bIsItemLabel)
                {
                    if (_strItemName != "" && _strItemNo != "")
                    {
                        //define dictionary for Item label 
                        Dictionary<string, string> objKeyValue = new Dictionary<string, string>();
                        objKeyValue.Add(Constants.LBL_ITEMNAME, _strItemName);
                        objKeyValue.Add(Constants.LBL_ITEMNO, _strItemNo);
                        ret = App.LabelPrinterObject.WriteLabel(Constants.LBL_ITEM, objKeyValue, 2);                        
                    }
                }
                else
                {
                    if (_strURL != "" && _strTextLine1 != "" && _strTextLine2 != "")
                    {
                        //define dictionary for QR label 
                        Dictionary<string, string> objKeyValue = new Dictionary<string, string>();
                        objKeyValue.Add(Constants.LBL_URL, _strURL);
                        objKeyValue.Add(Constants.LBL_TL1, _strTextLine1);
                        objKeyValue.Add(Constants.LBL_TL2, _strTextLine2);
                        ret = App.LabelPrinterObject.WriteLabel(Constants.LBL_URLQR, objKeyValue, 3);
                    }

                }

            }
            catch (Exception)
            {
               // SetStatusMsg(false, Constants.LBL_ERROR_STATUS);
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
                SystemTray.IsVisible = true;
                var prog = new ProgressIndicator();
                prog.Text = strValue;
                prog.IsVisible = true;
                prog.IsIndeterminate = false;
                if (value)
                    SystemTray.SetBackgroundColor(this, System.Windows.Media.Color.FromArgb(0, 0, 255, 0));
                else
                    SystemTray.SetBackgroundColor(this, System.Windows.Media.Color.FromArgb(0, 255, 0, 0));
                SystemTray.SetProgressIndicator(this, prog);
            }
           finally
            {
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// rbItemLabel_Checked 
        /// </summary>
        /// <remarks>
        /// rbItemLabel_Checked event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void rbItemLabel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.rbItemLabel.IsChecked == true)
                {
                    EnableURLControls(false);
                    EnableItemControls(true);
                }
                else
                    EnableItemControls(false);
            }
            catch (Exception)
            {
                if(null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }

        }
        /// ************************************************************************************************
        /// <summary>
        /// rbURLLabel_Checked 
        /// </summary>
        /// <remarks>
        /// rbURLLabel_Checked event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void rbURLLabel_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.rbURLLabel.IsChecked == true)
                {
                    EnableItemControls(false);
                    EnableURLControls(true);
                }
                else
                    EnableURLControls(false);
            }
            catch (Exception)
            {
                if(null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtItemNo_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtItemNo_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtItemNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strItemNo = this.txtItemNo.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtURL_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtURL_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strURL = this.txtURL.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtTextLine1_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtTextLine1_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtTextLine1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strTextLine1 = this.txtTextLine1.Text;
                UpdatePrintLabelBtn();
            }
            catch (Exception)
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }
        }
        /// ************************************************************************************************
        /// <summary>
        /// txtTextLine2_TextChanged 
        /// </summary>
        /// <remarks>
        /// txtTextLine2_TextChanged event
        /// <Development> Implemented. </Development>                                         
        /// ************************************************************************************************
        private void txtTextLine2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _strTextLine2 = this.txtTextLine2.Text;
                UpdatePrintLabelBtn();                
            }
            catch (Exception)
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("INVALID_ERR"));
            }
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
                    SetStatusMsg(true, _rm.GetString("LBL_PRINT_SUCCESS"));
            }
            else
            {
                if (null != _rm)
                    SetStatusMsg(false, _rm.GetString("LBL_ERROR_STATUS"));
            }
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
    }
}