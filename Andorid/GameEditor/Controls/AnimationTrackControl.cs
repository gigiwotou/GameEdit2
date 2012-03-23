using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameData;

namespace GameEditor.Controls
{
    public partial class AnimationTrackControl : UserControl
    {
        const int KeyButtonWidth = 10;
        AnimationControl mParent;
        AnimationTrack mAnimTrack;
        Button mCurrnetKeyButton;

        public AnimationTrackControl(AnimationControl parent, AnimationTrack animTrack)
        {
            InitializeComponent();

            mParent = parent;
            mAnimTrack = animTrack;

            CreateAnimKeys();
        }

        private void AnimationTrackControl_Load(object sender, EventArgs e)
        {
            CreateAnimKeys();
        }

        void CreateAnimKeys()
        {
            Controls.Clear();
            foreach (AnimationKey key in mAnimTrack.AnimKeys)
            {
                Button keyButton = new Button();
                keyButton.Location = Point.Empty;
                keyButton.Size = new Size(KeyButtonWidth, Size.Height - 3);

                keyButton.MouseDown += new MouseEventHandler(keyButton_MouseDown);
                keyButton.MouseMove += new MouseEventHandler(keyButton_MouseMove);
                keyButton.MouseUp += new MouseEventHandler(keyButton_MouseUp);
                keyButton.Tag = key;

                Controls.Add(keyButton);
            }

            Refresh();
        }
        
        void AddAnimKey(int time)
        {
            AnimationKey animKey = mAnimTrack.CachedKey.Clone();
            animKey.Time = time;

            mAnimTrack.AnimKeys.Add(animKey);
            mAnimTrack.AnimKeys.Sort(new AnimationKey.AnimationKeyCompare());

            CreateAnimKeys();
        }

        Point mLastMousePos = Point.Empty;
        void keyButton_MouseDown(object sender, MouseEventArgs e)
        {
            Button keyButton = sender as Button;
            mLastMousePos = keyButton.PointToScreen(e.Location);
            mKeyButtonMoved = false;
            AnimationKey key = keyButton.Tag as AnimationKey;
            mParent.SetTimePosition(key.Time);
        }

        bool mKeyButtonMoved = false;
        void keyButton_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            
            Button keyButton = sender as Button;
            Point currentPos = keyButton.PointToScreen(e.Location);
            int delta = currentPos.X - mLastMousePos.X;
            int newPosX = Math.Min(Math.Max(0, keyButton.Location.X + delta), Size.Width);
            if (newPosX != keyButton.Location.X)
            {
                keyButton.Location = new Point(newPosX, keyButton.Location.Y);
                mKeyButtonMoved = true;
            }

            mLastMousePos = currentPos;
        }

        void keyButton_MouseUp(object sender, MouseEventArgs e)
        {
            Button keyButton = sender as Button;
            if (e.Button == MouseButtons.Left)
            {
                if (mKeyButtonMoved)
                {
                    AnimationKey animKey = keyButton.Tag as AnimationKey;
                    animKey.Time = (int)(keyButton.Location.X / mParent.TimeToPixel + 0.5f);

                    CreateAnimKeys();

                    mKeyButtonMoved = false;
                }
                else if (e.Clicks == 2)
                {
                    mCurrnetKeyButton = keyButton;
                    OnKeyButtonEditClicked(sender, e);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                mCurrnetKeyButton = keyButton;
                contextMenuStrip1.Show(keyButton, e.Location);
            }
        }

        public override void Refresh()
        {
            foreach (Control control in Controls)
            {
                AnimationKey animKey = control.Tag as AnimationKey;
                int posX = (int)(animKey.Time * mParent.TimeToPixel + 0.5f);
                control.Location = new Point(posX, control.Location.Y);
            }

            base.Refresh();
        }

        private void OnMouseClicked(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                int time = (int)(e.Location.X / mParent.TimeToPixel + 0.5f);
                AddAnimKey(time);
            }
            else if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip2.Show(this, e.Location);
                contextMenuStrip2.Tag = e.Location;
            }
        }

        private void OnKeyButtonSetValueClicked(object sender, EventArgs e)
        {
            AnimationKey key = mCurrnetKeyButton.Tag as AnimationKey;

            int time = key.Time;
            key.CopyFrom(mAnimTrack.CachedKey);
            key.Time = time;
        }

        private void OnKeyButtonEditClicked(object sender, EventArgs e)
        {
            AnimationKey key = mCurrnetKeyButton.Tag as AnimationKey;
            SettingDlg dlg = new SettingDlg("Edit", key);
            dlg.ShowDialog();
        }

        private void OnKeyButtonDeleteClicked(object sender, EventArgs e)
        {
            mAnimTrack.AnimKeys.Remove(mCurrnetKeyButton.Tag as AnimationKey);
            CreateAnimKeys();
        }

        private void OnAddKeyClicked(object sender, EventArgs e)
        {
            int pos = ((Point)contextMenuStrip2.Tag).X;
            int time = (int)(pos / mParent.TimeToPixel + 0.5f);
            AddAnimKey(time);
        }
    }
}
