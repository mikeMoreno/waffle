﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Waffle.Lib;

namespace Waffle.History
{
    internal partial class HistoryForm : Form
    {
        private HistoryService HistoryService { get; }

        Guid? SelectedTabKey { get; }

        public delegate void LinkClickedEventHandler(object sender, LinkClickedEventArgs e);

        public event LinkClickedEventHandler LinkClicked;

        public HistoryForm(HistoryService historyService, Guid? selectedTabKey = null)
        {
            InitializeComponent();

            HistoryService = historyService;
            SelectedTabKey = selectedTabKey;

            ShowHistory();
        }

        private void ShowHistory()
        {
            var historyEntities = HistoryService.LoadHistory(SelectedTabKey);

            listHistoryEntities.DataSource = historyEntities;

            //listHistoryEntities.DisplayMember = 

            //foreach(var historyEntity in historyEntities)
            //{
            //    listHistoryEntities.Items.
            //}
        }

        private void listHistoryEntities_DoubleClick(object sender, EventArgs e)
        {
            var selectedItem = listHistoryEntities.SelectedItem;

            if(selectedItem == null)
            {
                return;
            }

            var historyEntity = selectedItem as HistoryEntity;

            LinkClicked?.Invoke(this, new LinkClickedEventArgs(new LinkLine(historyEntity.Url)));
        }
    }
}
