using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameEditor.Controls;
using GameData;

namespace GameEditor
{
    public partial class MainForm : Form
    {
        ImagesetPanel mImagesetPanel;
        AnimationSetPanel mAnimationSetPanel;
        MapBlockPanel mMapBlockPanel;

        public MainForm()
        {
            Root.Instance.Init(Application.StartupPath);

            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            mImagesetPanel = new ImagesetPanel();
            mImagesetPanel.Dock = DockStyle.Fill;
            tabPageImageset.Controls.Add(mImagesetPanel);

            mAnimationSetPanel = new AnimationSetPanel();
            mAnimationSetPanel.Dock = DockStyle.Fill;
            tabPageAnimations.Controls.Add(mAnimationSetPanel);

            mMapBlockPanel = new MapBlockPanel();
            mMapBlockPanel.Dock = DockStyle.Fill;
            tabPageMap.Controls.Add(mMapBlockPanel);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Root.Instance.Shutdown();
        }
    }
}
