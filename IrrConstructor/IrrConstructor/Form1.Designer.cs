namespace IrrConstructor
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panelRenderingWindow = new System.Windows.Forms.Panel();
            this.backgroundRendering = new System.ComponentModel.BackgroundWorker();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolStripButtonOpen = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonCollapseExpand = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonZoom = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMove = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateFirst = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonRotateAround = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonUpDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonResetCamera = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSkyVisible = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMapVisible = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonBotsVisible = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonAIVisible = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreateWaypoint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonMoveWaypoint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteWaypoint = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteWaypoints = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCreateEdge = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteEdge = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonDeleteEdges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonSetupGame = new System.Windows.Forms.ToolStripButton();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonLoadAI = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveAI = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRenderingWindow
            // 
            this.panelRenderingWindow.Location = new System.Drawing.Point(215, 23);
            this.panelRenderingWindow.Name = "panelRenderingWindow";
            this.panelRenderingWindow.Size = new System.Drawing.Size(520, 390);
            this.panelRenderingWindow.TabIndex = 10;
            this.panelRenderingWindow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelRenderingWindow_MouseDown);
            this.panelRenderingWindow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelRenderingWindow_MouseMove);
            this.panelRenderingWindow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelRenderingWindow_MouseUp);
            this.panelRenderingWindow.Move += new System.EventHandler(this.panelRenderingWindow_Move);
            // 
            // backgroundRendering
            // 
            this.backgroundRendering.WorkerReportsProgress = true;
            this.backgroundRendering.WorkerSupportsCancellation = true;
            this.backgroundRendering.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundRendering_DoWork);
            this.backgroundRendering.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundRendering_ProgressChanged);
            this.backgroundRendering.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundRendering_RunWorkerCompleted);
            // 
            // propertyGrid
            // 
            this.propertyGrid.Location = new System.Drawing.Point(3, 23);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(210, 390);
            this.propertyGrid.TabIndex = 41;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // toolStripButtonOpen
            // 
            this.toolStripButtonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonOpen.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonOpen.Image")));
            this.toolStripButtonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonOpen.Name = "toolStripButtonOpen";
            this.toolStripButtonOpen.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonOpen.Text = "Открыть";
            this.toolStripButtonOpen.Click += new System.EventHandler(this.toolStripButtonOpen_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSave.Text = "Сохранить";
            this.toolStripButtonSave.Click += new System.EventHandler(this.toolStripButtonSave_Click);
            // 
            // toolStripButtonPlay
            // 
            this.toolStripButtonPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPlay.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPlay.Image")));
            this.toolStripButtonPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPlay.Name = "toolStripButtonPlay";
            this.toolStripButtonPlay.Padding = new System.Windows.Forms.Padding(0, 0, 113, 0);
            this.toolStripButtonPlay.Size = new System.Drawing.Size(133, 22);
            this.toolStripButtonPlay.Text = "Запуск";
            this.toolStripButtonPlay.Click += new System.EventHandler(this.toolStripButtonPlay_Click);
            // 
            // toolStripButtonCollapseExpand
            // 
            this.toolStripButtonCollapseExpand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCollapseExpand.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCollapseExpand.Image")));
            this.toolStripButtonCollapseExpand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCollapseExpand.Name = "toolStripButtonCollapseExpand";
            this.toolStripButtonCollapseExpand.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCollapseExpand.Text = "Свернуть-развернуть";
            this.toolStripButtonCollapseExpand.Click += new System.EventHandler(this.toolStripButtonCollapseExpand_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonZoom
            // 
            this.toolStripButtonZoom.CheckOnClick = true;
            this.toolStripButtonZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonZoom.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonZoom.Image")));
            this.toolStripButtonZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonZoom.Name = "toolStripButtonZoom";
            this.toolStripButtonZoom.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonZoom.Text = "Приблизить";
            this.toolStripButtonZoom.Click += new System.EventHandler(this.toolStripButtonZoom_Click);
            // 
            // toolStripButtonMove
            // 
            this.toolStripButtonMove.CheckOnClick = true;
            this.toolStripButtonMove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMove.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMove.Image")));
            this.toolStripButtonMove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMove.Name = "toolStripButtonMove";
            this.toolStripButtonMove.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMove.Text = "Двигать";
            this.toolStripButtonMove.Click += new System.EventHandler(this.toolStripButtonMove_Click);
            // 
            // toolStripButtonRotateFirst
            // 
            this.toolStripButtonRotateFirst.CheckOnClick = true;
            this.toolStripButtonRotateFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotateFirst.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRotateFirst.Image")));
            this.toolStripButtonRotateFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateFirst.Name = "toolStripButtonRotateFirst";
            this.toolStripButtonRotateFirst.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRotateFirst.Text = "Вращать";
            this.toolStripButtonRotateFirst.Click += new System.EventHandler(this.toolStripButtonRotateFirst_Click);
            // 
            // toolStripButtonRotateAround
            // 
            this.toolStripButtonRotateAround.CheckOnClick = true;
            this.toolStripButtonRotateAround.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonRotateAround.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonRotateAround.Image")));
            this.toolStripButtonRotateAround.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonRotateAround.Name = "toolStripButtonRotateAround";
            this.toolStripButtonRotateAround.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonRotateAround.Text = "Вращать вокруг";
            this.toolStripButtonRotateAround.Click += new System.EventHandler(this.toolStripButtonRotateAround_Click);
            // 
            // toolStripButtonUpDown
            // 
            this.toolStripButtonUpDown.CheckOnClick = true;
            this.toolStripButtonUpDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonUpDown.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonUpDown.Image")));
            this.toolStripButtonUpDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonUpDown.Name = "toolStripButtonUpDown";
            this.toolStripButtonUpDown.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonUpDown.Text = "Вперед-назад";
            this.toolStripButtonUpDown.Click += new System.EventHandler(this.toolStripButtonUpDown_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonResetCamera
            // 
            this.toolStripButtonResetCamera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonResetCamera.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonResetCamera.Image")));
            this.toolStripButtonResetCamera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonResetCamera.Name = "toolStripButtonResetCamera";
            this.toolStripButtonResetCamera.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonResetCamera.Text = "Сбросить камеру";
            this.toolStripButtonResetCamera.Click += new System.EventHandler(this.toolStripButtonResetCamera_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSkyVisible
            // 
            this.toolStripButtonSkyVisible.CheckOnClick = true;
            this.toolStripButtonSkyVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSkyVisible.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSkyVisible.Image")));
            this.toolStripButtonSkyVisible.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSkyVisible.Name = "toolStripButtonSkyVisible";
            this.toolStripButtonSkyVisible.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSkyVisible.Text = "Небо";
            // 
            // toolStripButtonMapVisible
            // 
            this.toolStripButtonMapVisible.CheckOnClick = true;
            this.toolStripButtonMapVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMapVisible.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMapVisible.Image")));
            this.toolStripButtonMapVisible.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMapVisible.Name = "toolStripButtonMapVisible";
            this.toolStripButtonMapVisible.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMapVisible.Text = "Карта";
            // 
            // toolStripButtonBotsVisible
            // 
            this.toolStripButtonBotsVisible.CheckOnClick = true;
            this.toolStripButtonBotsVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonBotsVisible.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonBotsVisible.Image")));
            this.toolStripButtonBotsVisible.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonBotsVisible.Name = "toolStripButtonBotsVisible";
            this.toolStripButtonBotsVisible.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonBotsVisible.Text = "Боты";
            // 
            // toolStripButtonAIVisible
            // 
            this.toolStripButtonAIVisible.CheckOnClick = true;
            this.toolStripButtonAIVisible.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonAIVisible.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonAIVisible.Image")));
            this.toolStripButtonAIVisible.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonAIVisible.Name = "toolStripButtonAIVisible";
            this.toolStripButtonAIVisible.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonAIVisible.Text = "ИИ";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCreateWaypoint
            // 
            this.toolStripButtonCreateWaypoint.CheckOnClick = true;
            this.toolStripButtonCreateWaypoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateWaypoint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCreateWaypoint.Image")));
            this.toolStripButtonCreateWaypoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateWaypoint.Name = "toolStripButtonCreateWaypoint";
            this.toolStripButtonCreateWaypoint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateWaypoint.Text = "Создать точку";
            this.toolStripButtonCreateWaypoint.Click += new System.EventHandler(this.toolStripButtonCreateWaypoint_Click);
            // 
            // toolStripButtonMoveWaypoint
            // 
            this.toolStripButtonMoveWaypoint.CheckOnClick = true;
            this.toolStripButtonMoveWaypoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonMoveWaypoint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonMoveWaypoint.Image")));
            this.toolStripButtonMoveWaypoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonMoveWaypoint.Name = "toolStripButtonMoveWaypoint";
            this.toolStripButtonMoveWaypoint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonMoveWaypoint.Text = "Двигать точку";
            this.toolStripButtonMoveWaypoint.Click += new System.EventHandler(this.toolStripButtonMoveWaypoint_Click);
            // 
            // toolStripButtonDeleteWaypoint
            // 
            this.toolStripButtonDeleteWaypoint.CheckOnClick = true;
            this.toolStripButtonDeleteWaypoint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteWaypoint.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteWaypoint.Image")));
            this.toolStripButtonDeleteWaypoint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteWaypoint.Name = "toolStripButtonDeleteWaypoint";
            this.toolStripButtonDeleteWaypoint.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteWaypoint.Text = "Удалить точку";
            this.toolStripButtonDeleteWaypoint.Click += new System.EventHandler(this.toolStripButtonDeleteWaypoint_Click);
            // 
            // toolStripButtonDeleteWaypoints
            // 
            this.toolStripButtonDeleteWaypoints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteWaypoints.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteWaypoints.Image")));
            this.toolStripButtonDeleteWaypoints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteWaypoints.Name = "toolStripButtonDeleteWaypoints";
            this.toolStripButtonDeleteWaypoints.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteWaypoints.Text = "Удалить точки";
            this.toolStripButtonDeleteWaypoints.Click += new System.EventHandler(this.toolStripButtonDeleteWaypoints_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCreateEdge
            // 
            this.toolStripButtonCreateEdge.CheckOnClick = true;
            this.toolStripButtonCreateEdge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonCreateEdge.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCreateEdge.Image")));
            this.toolStripButtonCreateEdge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCreateEdge.Name = "toolStripButtonCreateEdge";
            this.toolStripButtonCreateEdge.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonCreateEdge.Text = "Создать ребро";
            this.toolStripButtonCreateEdge.Click += new System.EventHandler(this.toolStripButtonCreateEdge_Click);
            // 
            // toolStripButtonDeleteEdge
            // 
            this.toolStripButtonDeleteEdge.CheckOnClick = true;
            this.toolStripButtonDeleteEdge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteEdge.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteEdge.Image")));
            this.toolStripButtonDeleteEdge.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteEdge.Name = "toolStripButtonDeleteEdge";
            this.toolStripButtonDeleteEdge.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteEdge.Text = "Удалить ребро";
            this.toolStripButtonDeleteEdge.Click += new System.EventHandler(this.toolStripButtonDeleteEdge_Click);
            // 
            // toolStripButtonDeleteEdges
            // 
            this.toolStripButtonDeleteEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonDeleteEdges.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonDeleteEdges.Image")));
            this.toolStripButtonDeleteEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonDeleteEdges.Name = "toolStripButtonDeleteEdges";
            this.toolStripButtonDeleteEdges.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonDeleteEdges.Text = "Удалить ребра";
            this.toolStripButtonDeleteEdges.Click += new System.EventHandler(this.toolStripButtonDeleteEdges_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonSetupGame
            // 
            this.toolStripButtonSetupGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSetupGame.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSetupGame.Image")));
            this.toolStripButtonSetupGame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSetupGame.Name = "toolStripButtonSetupGame";
            this.toolStripButtonSetupGame.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSetupGame.Text = "Установить";
            this.toolStripButtonSetupGame.Click += new System.EventHandler(this.toolStripButtonSetupGame_Click);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonOpen,
            this.toolStripButtonSave,
            this.toolStripButtonPlay,
            this.toolStripButtonCollapseExpand,
            this.toolStripSeparator1,
            this.toolStripButtonZoom,
            this.toolStripButtonMove,
            this.toolStripButtonRotateFirst,
            this.toolStripButtonRotateAround,
            this.toolStripButtonUpDown,
            this.toolStripSeparator2,
            this.toolStripButtonResetCamera,
            this.toolStripSeparator6,
            this.toolStripButtonSkyVisible,
            this.toolStripButtonMapVisible,
            this.toolStripButtonBotsVisible,
            this.toolStripButtonAIVisible,
            this.toolStripSeparator3,
            this.toolStripButtonLoadAI,
            this.toolStripButtonSaveAI,
            this.toolStripSeparator7,
            this.toolStripButtonCreateWaypoint,
            this.toolStripButtonMoveWaypoint,
            this.toolStripButtonDeleteWaypoint,
            this.toolStripButtonDeleteWaypoints,
            this.toolStripSeparator5,
            this.toolStripButtonCreateEdge,
            this.toolStripButtonDeleteEdge,
            this.toolStripButtonDeleteEdges,
            this.toolStripSeparator4,
            this.toolStripButtonSetupGame});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(736, 25);
            this.toolStrip.TabIndex = 44;
            this.toolStrip.Text = "toolStrip1";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 415);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(736, 22);
            this.statusStrip.TabIndex = 45;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonLoadAI
            // 
            this.toolStripButtonLoadAI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLoadAI.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLoadAI.Image")));
            this.toolStripButtonLoadAI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLoadAI.Name = "toolStripButtonLoadAI";
            this.toolStripButtonLoadAI.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonLoadAI.Text = "Загрузить ИИ";
            this.toolStripButtonLoadAI.Click += new System.EventHandler(this.toolStripButtonLoadAI_Click);
            // 
            // toolStripButtonSaveAI
            // 
            this.toolStripButtonSaveAI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonSaveAI.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveAI.Image")));
            this.toolStripButtonSaveAI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveAI.Name = "toolStripButtonSaveAI";
            this.toolStripButtonSaveAI.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonSaveAI.Text = "Сохранить ИИ";
            this.toolStripButtonSaveAI.Click += new System.EventHandler(this.toolStripButtonSaveAI_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 437);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.panelRenderingWindow);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(752, 475);
            this.Name = "Form1";
            this.Text = "IrrConstructor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRenderingWindow;
        private System.ComponentModel.BackgroundWorker backgroundRendering;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.ToolStripButton toolStripButtonOpen;
        private System.Windows.Forms.ToolStripButton toolStripButtonCollapseExpand;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripButton toolStripButtonSetupGame;
        private System.Windows.Forms.ToolStripButton toolStripButtonPlay;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonZoom;
        private System.Windows.Forms.ToolStripButton toolStripButtonMove;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateFirst;
        private System.Windows.Forms.ToolStripButton toolStripButtonRotateAround;
        private System.Windows.Forms.ToolStripButton toolStripButtonUpDown;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonAIVisible;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButtonBotsVisible;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateWaypoint;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolStripButtonMapVisible;
        private System.Windows.Forms.ToolStripButton toolStripButtonSkyVisible;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteWaypoint;
        private System.Windows.Forms.ToolStripButton toolStripButtonCreateEdge;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteWaypoints;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButtonResetCamera;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolStripButtonMoveWaypoint;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteEdge;
        private System.Windows.Forms.ToolStripButton toolStripButtonDeleteEdges;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripButton toolStripButtonLoadAI;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveAI;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    }
}

