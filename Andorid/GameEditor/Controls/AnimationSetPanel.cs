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
    public partial class AnimationSetPanel : UserControl
    {
        AnimationControl mAnimControl;

        public AnimationSetPanel()
        {
            InitializeComponent();
            
            mAnimControl = new AnimationControl();
            mAnimControl.Dock = DockStyle.Fill;
            groupBox3.Controls.Add(mAnimControl);

            mAnimControl.Enabled = false;

            ShowAnimationSets();
        }

        private void OnNewAnimSetClicked(object sender, EventArgs e)
        {
            AnimationSet animationSet = new AnimationSet();
            animationSet.Name = "AnimationSet" + AnimationSetManager.Instance.AnimationSets.Count;

            SettingDlg dlg = new SettingDlg("New Animation Set", animationSet);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (AnimationSetManager.Instance.Contains(animationSet.Name))
            {
                MessageBox.Show(string.Format("Animation '{0}' already exist!!!", animationSet.Name));
                return;
            }

            AnimationSetManager.Instance.Add(animationSet);

            ShowAnimationSets();

            listView1.Items[listView1.Items.Count - 1].Selected = true;
        }

        private void OnDeleteAnimSetClicked(object sender, EventArgs e)
        {
            foreach (ListViewItem lvItem in listView1.SelectedItems)
            {
                AnimationSet animationSet = lvItem.Tag as AnimationSet;
                AnimationSetManager.Instance.Delete(animationSet.Name);
            }

            ShowAnimationSets();
        }

        private void OnSaveAnimSetClicked(object sender, EventArgs e)
        {
            AnimationSetManager.Instance.Save();
        }

        private void OnNewAnimClicked(object sender, EventArgs e)
        {
            AnimationSet animationSet = listView2.Tag as AnimationSet;
            if (animationSet == null)
                return;

            Animation animation = new Animation();
            animation.Name = "Animation" + animationSet.Animations.Count;

            SettingDlg dlg = new SettingDlg("New Animation", animation);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            animationSet.Animations.Add(animation);

            ShowAnimationSet(animationSet);

            listView2.Items[listView2.Items.Count - 1].Selected = true;
        }

        private void OnDeleteAnimClicked(object sender, EventArgs e)
        {
            AnimationSet animationSet = listView2.Tag as AnimationSet;
            if (animationSet == null)
                return;

            foreach (ListViewItem lvItem in listView2.SelectedItems)
            {
                Animation animation = lvItem.Tag as Animation;
                animationSet.Animations.Remove(animation);
            }

            ShowAnimationSet(animationSet);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
                return;
            ShowAnimationSet(listView1.SelectedItems[0].Tag as AnimationSet);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count < 1)
                ShowAnimation(null);

            Animation anim = listView2.SelectedItems.Count > 0 ? listView2.SelectedItems[0].Tag as Animation : null;
            ShowAnimation(anim);
        }
        
        private void OnListViewDoubleClicked(object sender, EventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView == null || listView.SelectedItems.Count == 0)
                return;

            Object select = listView.SelectedItems[0].Tag;
            if (select == null)
                return;

            SettingDlg dlg = new SettingDlg("Edit", select);
            dlg.ShowDialog();
        }

        void ShowAnimationSets()
        {
            listView1.Items.Clear();
            foreach (AnimationSet animSet in AnimationSetManager.Instance.AnimationSets.Values)
            {
                ListViewItem lvItem = listView1.Items.Add(animSet.Name);
                lvItem.Tag = animSet;
            }
        }

        void ShowAnimationSet(AnimationSet animSet)
        {
            listView2.Items.Clear();
            listView2.Tag = animSet;

            foreach (Animation anim in animSet.Animations)
            {
                ListViewItem lvItem = listView2.Items.Add(anim.Name);
                lvItem.Tag = anim;
            }
        }

        void ShowAnimation(Animation anim)
        {
            mAnimControl.EditAnimation(anim);
        }
    }
}
