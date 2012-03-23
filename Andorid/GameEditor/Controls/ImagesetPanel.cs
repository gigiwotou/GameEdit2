using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GameData;
using GameEditor.Data;
using System.IO;

namespace GameEditor.Controls
{
    public partial class ImagesetPanel : UserControl
    {
        public ImagesetPanel()
        {
            InitializeComponent();
        }

        private void ImagesetPanel_Load(object sender, EventArgs e)
        {
            LoadImagesets();
        }

        void LoadImagesets()
        {
            Dictionary<string, Imageset> imagesets = ImagesetManager.Instance.Imagesets;

            listView1.Items.Clear();
            foreach (Imageset imageset in imagesets.Values)
            {
                ListViewItem lvItem = listView1.Items.Add(imageset.Name);
                lvItem.SubItems.Add(imageset.FileName);
                lvItem.SubItems.Add(imageset.Width.ToString());
                lvItem.SubItems.Add(imageset.Height.ToString());
                lvItem.Tag = imageset;

                LoadImageset(imageset);
            }
        }

        void LoadImageset(Imageset imageset)
        {
            try
            {
                if (imageset.Tag == null)
                    imageset.Tag = System.Drawing.Image.FromFile(imageset.FileName);

                foreach (GameData.Image image in imageset.Images)
                    image.Imageset = imageset;
            }
            catch (Exception)
            {
            }
        }

        void ShowImageset(Imageset imageset)
        {
            listView2.Items.Clear();

            foreach (GameData.Image image in imageset.Images)
            {
                ListViewItem lvItem = listView2.Items.Add(image.Name);
                lvItem.SubItems.Add(image.X.ToString());
                lvItem.SubItems.Add(image.Y.ToString());
                lvItem.SubItems.Add(image.Width.ToString());
                lvItem.SubItems.Add(image.Height.ToString());

                lvItem.Tag = image;
            }

            try
            {
                System.Drawing.Image texture = System.Drawing.Image.FromFile(imageset.FileName);

                panel1.Size = texture.Size;
                panel1.BackgroundImage = texture;
                panel1.Tag = imageset;
            }
            catch (Exception)
            {
            }
        }

        void ShowImage(GameData.Image image)
        {
            try
            {
                propertyGrid1.SelectedObject = image;
                propertyGrid1.Tag = image;

                if (panel1.BackgroundImage == null)
                    return;

                Bitmap srcBitmap = new Bitmap(panel1.BackgroundImage);
                Rectangle rect = new Rectangle(image.X, image.Y, image.Width, image.Height);
                panel2.BackgroundImage = srcBitmap.Clone(rect, srcBitmap.PixelFormat);
                panel2.Size = panel2.BackgroundImage.Size;

                boundPanel.Visible = true;
                boundPanel.Location = new Point(image.X, image.Y);
                boundPanel.Size = new Size(image.Width, image.Height);
            }
            catch (Exception)
            {
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count < 1)
                return;

            ListViewItem lvItem = listView1.SelectedItems[0];
            Imageset imageset = lvItem.Tag as Imageset;
            if (imageset == null)
                return;

            ShowImageset(imageset);
        }

        private void OnImportImageClicked(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                System.Drawing.Image texture = System.Drawing.Image.FromFile(openFileDialog1.FileName);
                String name = Path.GetFileName(openFileDialog1.FileName);
                if (name.LastIndexOf('.') > 0)
                    name = name.Substring(0, name.LastIndexOf('.'));

                ImagesetInfo info = new ImagesetInfo(name, openFileDialog1.FileName);
                SettingDlg dlg = new SettingDlg("Import image", info);
                if (dlg.ShowDialog() != DialogResult.OK)
                    return;

                if (ImagesetManager.Instance.Contains(info.Name))
                    throw new Exception(String.Format("The image name '{0}' already exist!!!", info.Name));

                Imageset imageset = new Imageset();
                imageset.Name = info.Name;
                imageset.FileName = info.FileName;
                imageset.Width = texture.Width;
                imageset.Height = texture.Height;
                imageset.Tag = texture;
                ImagesetManager.Instance.Add(imageset);

                LoadImagesets();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Import failed!!!");
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e)
        {
            foreach (ListViewItem lvItem in listView1.Items)
            {
                Imageset imageset = lvItem.Tag as Imageset;
                ImagesetManager.Instance.Delete(imageset.Name);
            }

            LoadImagesets();
        }

        void NewImage(Imageset imageset, GameData.Image image)
        {
            SettingDlg dlg = new SettingDlg("New image", image);
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            if (imageset.Contains(image.Name))
            {
                MessageBox.Show(String.Format("The image '{0}' already exist!!", image.Name));
                return;
            }

            imageset.Add(image);
            image.Imageset = imageset;

            ShowImageset(imageset);

            listView2.Items[listView2.Items.Count - 1].Selected = true;
        }

        private void OnNewImageClicked(object sender, EventArgs e)
        {
            GameData.Image image = new GameData.Image();
            image.Name = "NewImage";
            NewImage(panel1.Tag as Imageset, image);
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count < 1)
                return;

            GameData.Image image = listView2.SelectedItems[0].Tag as GameData.Image;
            if (image == null)
                return;

            ShowImage(image);
        }

        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            GameData.Image image = propertyGrid1.SelectedObject as GameData.Image;
            if (image == null)
                return;
            ShowImage(image);
        }

        private void OnSaveImagesetClick(object sender, EventArgs e)
        {
            ImagesetManager.Instance.Save();
        }

        private void OnDoubleClicked(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 1)
                return;

            SettingDlg dlg = new SettingDlg("Edit", listView1.SelectedItems[0].Tag);
            dlg.ShowDialog();
        }

        private void OnCutImageClicked(object sender, EventArgs e)
        {
            panel1.Capture = true;
        }

        Point mLastMousePos = Point.Empty;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            mLastMousePos = e.Location;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!panel1.Capture || e.Button != MouseButtons.Left)
                return;

            if (e.Location == mLastMousePos)
                return;

            if (e.Location.X > panel1.Width || e.Location.Y > panel1.Height)
                return;

            int width = e.Location.X - mLastMousePos.X;
            int height = e.Location.Y - mLastMousePos.Y;
            if (width <= 0 || height <= 0)
                return;

            boundPanel.Visible = true;
            boundPanel.Size = new Size(width, height);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            if (panel1.Capture)
            {
                GameData.Image image = new GameData.Image();
                image.Name = "NewImage";
                image.X = boundPanel.Location.X;
                image.Y = boundPanel.Location.Y;
                image.Width = boundPanel.Width;
                image.Height = boundPanel.Height;

                NewImage(panel1.Tag as Imageset, image);
            }

            boundPanel.Visible = false;
            panel1.Capture = false;
        }
    }
}
