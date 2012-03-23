using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameData;
using System.IO;

namespace GameEditor.Controls
{
    public partial class AnimationControl : UserControl
    {
        AnimationPreviewPanel mPreviewPanel;
        float mTimeToPixel = 0.1f;
        Animation mAnimation;
        int mStartTime = 0;
        int mEndTime = 10000;

        public float TimeToPixel { get { return mTimeToPixel; } set { mTimeToPixel = value; } }

        public AnimationControl()
        {
            InitializeComponent();

            mPreviewPanel = new AnimationPreviewPanel(this);
            mPreviewPanel.Dock = DockStyle.Fill;

            splitContainer2.Panel1.Controls.Add(mPreviewPanel);
        }

        public void EditAnimation(Animation anim)
        {
            mAnimation = anim;
            if (mAnimation != null)
            {
                panel1.Size = new Size((int)(anim.Time * TimeToPixel + 0.5f), panel1.Size.Height);

                ShowAnimTracks();
                Enabled = true;
            }
            else
                Enabled = false;

            mPreviewPanel.EditAnimation(anim);
        }

        void ShowAnimTracks()
        {
            treeView1.Nodes.Clear();
            panel1.Controls.Clear();

            ShowAnimTracks(treeView1.Nodes, mAnimation.AnimTracks);
        }

        AnimationTrackControl GetAnimTrackControl(AnimationTrack track)
        {
            foreach (Control control in panel1.Controls)
            {
                if (control.Tag == track)
                    return control as AnimationTrackControl;
            }
            return null;
        }

        void HideControl(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                AnimationTrackControl control = GetAnimTrackControl(node.Tag as AnimationTrack);
                if (control != null)
                    control.Visible = false;
            }
        }

        void Refresh(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                int nodePosY = node.Bounds.Y;
                AnimationTrackControl control = GetAnimTrackControl(node.Tag as AnimationTrack);
                if (control != null)
                {
                    control.Visible = true;
                    control.Location = new Point(control.Location.X, nodePosY - 1);
                }

                if (node.IsExpanded)
                    Refresh(node.Nodes);
                else
                    HideControl(node.Nodes);
            }
        }

        public override void Refresh()
        {
            Refresh(treeView1.Nodes);

            base.Refresh();

            SetTimePosition(mPreviewPanel.TimePosition);
        }

        void ShowAnimTracks(TreeNodeCollection nodes, List<AnimationTrack> tracks)
        {
            nodes.Clear();
            foreach (AnimationTrack track in tracks)
                ShowAnimTracks(nodes, track);
        }

        void ShowAnimTracks(TreeNodeCollection nodes, AnimationTrack track)
        {
            TreeNode node = nodes.Add(track.Name);
            node.Checked = track.Enabled;
            node.Tag = track;

            int nodeY = node.Bounds.Y;
            int nodeHeight = node.TreeView.ItemHeight;

            AnimationTrackControl trackControl = new AnimationTrackControl(this, track);
            trackControl.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
            trackControl.Location = new Point(0, nodeY - 1);
            trackControl.Size = new Size(panel1.Width, nodeHeight + 1);
            trackControl.Tag = track;

            panel1.Controls.Add(trackControl);

            track.FlushImage();

            if (track.AnimTracks.Count > 0)
                ShowAnimTracks(node.Nodes, track.AnimTracks);
        }

        private void OnAddAnimTrackClicked(object sender, EventArgs e)
        {
            AnimationTrack newTrack = new AnimationTrack();
            newTrack.Name = "NewTrack" + mAnimation.AnimTracks.Count;

            if (treeView1.SelectedNode != null)
            {
                AnimationTrack parent = treeView1.SelectedNode.Tag as AnimationTrack;
                parent.AnimTracks.Add(newTrack);

                ShowAnimTracks(treeView1.SelectedNode.Nodes, newTrack);
            }
            else
            {
                mAnimation.AnimTracks.Add(newTrack);

                ShowAnimTracks(treeView1.Nodes, newTrack);
            }

            Refresh();
        }

        private void OnDeleteAnimTrackClicked(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {

        }

        private void treeView1_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            Refresh();
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            Refresh();
        }

        int TimeToPixelPos(int time)
        {
            return panel1.Location.X + (int)(time * TimeToPixel + 0.5f);
        }

        int PixelToTimePos(int pixel)
        {
            return (int)((pixel - panel1.Location.X) / TimeToPixel + 0.5f);
        }

        Pen mCurPen = new Pen(Color.Black);
        private void OnTimeLinePaint(object sender, PaintEventArgs e)
        {
            Panel panel = panel1;
            for (int cur = mStartTime; cur <= mEndTime; cur += 100)
            {
                int pixel = TimeToPixelPos(cur);
                if (pixel > panel.Location.X + panel.Size.Width)
                    break;

                int curHeight = 5;
                if ((cur % 1000) == 0)
                    curHeight = 12;
                else if ((cur % 500) == 0)
                    curHeight = 8;

                e.Graphics.DrawLine(mCurPen, 
                    new Point(pixel, 0),
                    new Point(pixel, curHeight));
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeView1.SelectedNode == null)
                return;

            SettingDlg dlg = new SettingDlg("Edit", treeView1.SelectedNode.Tag);
            dlg.ShowDialog();
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                AnimationTrackControl control = GetAnimTrackControl(treeView1.SelectedNode.Tag as AnimationTrack);
                if (control != null)
                    control.BorderStyle = BorderStyle.FixedSingle;
            }

            AnimationTrack track = e.Node.Tag as AnimationTrack;
            AnimationTrackControl selected = GetAnimTrackControl(track);
            if (selected != null)
                selected.BorderStyle = BorderStyle.Fixed3D;

            mPreviewPanel.SelectAnimationTrack(mAnimation, track);
        }

        TreeNode SelectAnimationTrack(TreeNodeCollection nodes, AnimationTrack track)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Tag == track)
                    return node;

                if (node.Nodes.Count > 0)
                {
                    TreeNode childNode = SelectAnimationTrack(node.Nodes, track);
                    if (childNode != null)
                        return childNode;
                }
            }
            return null;
        }

        public void SelectAnimationTrack(AnimationTrack track)
        {
            TreeNode node = SelectAnimationTrack(treeView1.Nodes, track);
            if (node != null)
                treeView1.SelectedNode = node;
        }

        void UpdateUIControls()
        {
            toolStripButtonPlay.Enabled = !timer1.Enabled;
            toolStripButtonPause.Enabled = timer1.Enabled;
            toolStripButtonStop.Enabled = (mPreviewPanel.TimePosition != 0);
        }

        public void SetTimePosition(int time)
        {
            mPreviewPanel.SetTimePosition(time);

            int pos = TimeToPixelPos(time);
            buttonTime.Location = new Point(pos, buttonTime.Location.Y);
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            UpdateUIControls();

            toolStripButtonStop.Enabled = true;
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            UpdateUIControls();
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            SetTimePosition(0);

            timer1.Enabled = false;
            UpdateUIControls();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (mAnimation == null)
                return;

            int newTime = mPreviewPanel.TimePosition + timer1.Interval;
            if (newTime > mAnimation.Time)
                newTime -= mAnimation.Time;
            SetTimePosition(newTime);
        }

        private void OnTimePanelMouseDown(object sender, MouseEventArgs e)
        {
            SetTimePosition(PixelToTimePos(e.Location.X));
        }
    }
}
