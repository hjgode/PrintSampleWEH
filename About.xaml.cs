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

namespace PrintSample
{
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }
        public void Init(String name, String Version)
        {
            tbName.Text = name;
            tbVersion.Text = Version;
        }
    }
}
