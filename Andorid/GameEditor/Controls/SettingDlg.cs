using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameEditor.Controls
{
    public partial class SettingDlg : Form
    {
        public SettingDlg(string caption, Object obj)
        {
            InitializeComponent();

            Text = caption;

            propertyGrid1.SelectedObject = obj;
        }
    }
}
