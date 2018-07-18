﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhotoFrame.Domain.Model;
using PhotoFrame.Application;

namespace PhotoFrameApp
{
    public partial class SlideShow : Form
    {

        private IEnumerable<PhotoFrame.Domain.Model.Photo> photo_listview;//リストビュー上のフォト
        private IEnumerable<PhotoFrame.Domain.Model.Photo> slideshow_list;//スライドショーを行う写真リスト

        private IEnumerable<Album> albumlist;
        private PhotoFrameApplicationTest application;
        private int slideindex;

        public SlideShow(IEnumerable<PhotoFrame.Domain.Model.Photo> photolist, PhotoFrameApplicationTest application )
        {
            InitializeComponent();

            this.application = application;
            this.photo_listview = photolist;
            this.slideshow_list = photolist;
            
            this.albumlist = application.GetAllAlbums();
            this.slideindex = 0;
            timer_SlideShow.Interval = 3000;

            button_Back.Enabled = false;

            foreach (var albums in this.albumlist)
            {
                comboBox_AlbumName.Items.Add(albums.Name);
            }
            
            if (this.photo_listview != null)
            {
                pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;
            }
            else
            {
                button_Next.Enabled = false;
                button_Back.Enabled = false;
                radioButton_ListViewSlideShow.Enabled = false;
                radioButton_ListViewSlideShow.Checked = false;
                radioButton_AlbumSlideShow.Checked = true;
                pictureBox_SlideShow.ImageLocation = @"C:\研修用\写真がなし.png";
            }

        }

        /// <summary>
        /// 「保存済みのアルバム」のラジオボタンが変化したら//
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_AlbumSlideShow_CheckedChanged(object sender, EventArgs e)
        {
            this.slideindex = 0;

            if (radioButton_AlbumSlideShow.Checked == true)
            {//「保存済みのアルバム」のラジオボタンのチェックが入ったとき
                comboBox_AlbumName.Enabled = true;
                radioButton_ListViewSlideShow.Checked = false;
            }
        }


        /// <summary>
        /// 「リストビュー」のラジオボタンが変化したら
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_ListViewSlideShow_CheckedChanged(object sender, EventArgs e)
        {
            

            //「リストビューの」ラジオボタンのチェックが入ったとき
            if (radioButton_ListViewSlideShow.Checked == true && this.photo_listview != null)
            {
                this.slideindex = 0;
                this.slideshow_list = this.photo_listview;
                comboBox_AlbumName.Enabled = false;
                radioButton_AlbumSlideShow.Checked = false;
                //pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;

                Stop();

            }
        }

        /// <summary>
        /// コンボボックスのアルバム名が変わると、slideshow_listを更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox_AlbumName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.slideshow_list = application.SearchAlbum(comboBox_AlbumName.Text);
            pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;
            Stop();

        }

        /// <summary>
        /// スライドショーにアルバム名を付けて保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SaveAlbumName_Click(object sender, EventArgs e)
        {
            string savaName = textBox_SaveAlbumName.Text;
            //var list = (from p in this.albumlist where p.Name == savaName select p).ToList();


            if (this.photo_listview  == null)
            {
                MessageBox.Show("リストビューに写真がありません。");
            }
            else
            {
                if (savaName == "")
                {
                    MessageBox.Show("アルバム名を入力してください。");

                }
                else
                {
                    bool saveSuccess = application.AddAlbum(savaName, this.slideshow_list);
                    if (saveSuccess == true)
                    {
                        MessageBox.Show($"{savaName}というアルバム名で保存しました。");
                        comboBox_AlbumName.Items.Insert(0, savaName);

                    }
                    else if (saveSuccess == false)
                    {
                        MessageBox.Show("すでに保存されているアルバム名です。");

                    }

                }
            }
            
            

        }

        /// <summary>
        /// 再生ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_StartSlideShow_Click(object sender, EventArgs e)
        {
            Play();
        }

        /// <summary>
        /// 一時停止ボタンが押されとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Pause_Click(object sender, EventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// 停止ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Stop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// 「Back」ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Back_Click(object sender, EventArgs e)
        {

            if (this.slideindex > 0)
            {
                slideindex--;
                pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;

            }

            ScreenTransition();

        }

        /// <summary>
        /// 「Next」ボタンが押されたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Next_Click(object sender, EventArgs e)
        {
            if (this.slideindex < slideshow_list.Count() - 1)
            {
                slideindex++;
                pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;
            }

            ScreenTransition();

        }

        /// <summary>
        /// スライドショー再生中の挙動
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_SlideShow_Tick(object sender, EventArgs e)
        {
            if (slideindex < slideshow_list.Count()-1)
            {
                slideindex++;
                pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;
            }
            else
            {
                Stop();

            }

        }

        /// <summary>
        /// スライドショーを再生状態にする
        /// </summary>
        private void Play()
        {
            timer_SlideShow.Start();
            button_StartSlideShow.Enabled = false;
            button_Pause_SlideShow.Enabled = true;
            button_StopSlideShow.Enabled = true;

            radioButton_ListViewSlideShow.Enabled = false;
            radioButton_AlbumSlideShow.Enabled = false;
            comboBox_AlbumName.Enabled = false;
            textBox_SaveAlbumName.Enabled = false;
            button_SaveAlbumName.Enabled = false;
            button_Next.Enabled = false;
            button_Back.Enabled = false;
        }

        /// <summary>
        /// スライドショーを一時停止する 
        /// </summary>
        private void Pause()
        {
            timer_SlideShow.Stop();
            button_StartSlideShow.Enabled = true;
            button_Pause_SlideShow.Enabled = false;
            button_StopSlideShow.Enabled = true;

            radioButton_ListViewSlideShow.Enabled = true;
            radioButton_AlbumSlideShow.Enabled = true;
            comboBox_AlbumName.Enabled = true;
            textBox_SaveAlbumName.Enabled = true;
            button_SaveAlbumName.Enabled = true;
            button_Next.Enabled = true;
            button_Back.Enabled = true;

            if (slideindex == 0)
            {
                button_Back.Enabled = false;
                button_Next.Enabled = true;
            }
            else if (slideindex == this.slideshow_list.Count() - 1)
            {
                button_Back.Enabled = true;
                button_Next.Enabled = false;
            }
            else
            {
                button_Back.Enabled = true;
                button_Next.Enabled = true;

            }
        }

        /// <summary>
        /// スライドショーを停止する
        /// </summary>
        private void Stop()
        {

            slideindex = 0;
            timer_SlideShow.Stop();
            button_StartSlideShow.Enabled = true;
            button_Pause_SlideShow.Enabled = false;
            button_StopSlideShow.Enabled = false;

            if (this.photo_listview != null)
            {
                radioButton_ListViewSlideShow.Enabled = true;
            }
            radioButton_AlbumSlideShow.Enabled = true;
            comboBox_AlbumName.Enabled = true;
            textBox_SaveAlbumName.Enabled = true;
            button_SaveAlbumName.Enabled = true;
            button_Next.Enabled = true;
            button_Back.Enabled = true;

            button_Back.Enabled = false;
            button_Next.Enabled = true;


            //最初の画像をセット

            if (this.slideshow_list != null)
            {
                pictureBox_SlideShow.ImageLocation = this.slideshow_list.ElementAt(slideindex).File.FilePath;
            }
            else
            {
                pictureBox_SlideShow.ImageLocation = @"C:\研修用\写真がなし.png";

            }
        }

        private void ScreenTransition()
        {
            if (slideindex == 0)
            {
                button_Back.Enabled = false;
                button_Next.Enabled = true;
                button_Pause_SlideShow.Enabled = false;
                button_StopSlideShow.Enabled = false;
            }
            else if (slideindex == this.slideshow_list.Count() - 1)
            {
                button_Back.Enabled = true;
                button_Next.Enabled = false;
                button_Pause_SlideShow.Enabled = false;
                button_StopSlideShow.Enabled = true;

            }
            else
            {
                button_Back.Enabled = true;
                button_Next.Enabled = true;

                button_Pause_SlideShow.Enabled = false;
                button_StopSlideShow.Enabled = true;


            }

        }
    }
}
