namespace QueueCalc;

public partial class MainForm : Form
{
    // ── state
    bool arrIsM = true;
    bool svcIsM = true;
    string arrSub = "gamma";
    string svcSub = "gamma";

    // ── colours
    readonly Color C_BG      = Color.FromArgb(28,  28,  28);
    readonly Color C_PANEL   = Color.FromArgb(40,  40,  42);
    readonly Color C_CARD    = Color.FromArgb(50,  50,  53);
    readonly Color C_INPUT   = Color.FromArgb(62,  62,  65);
    readonly Color C_BORDER  = Color.FromArgb(70,  70,  70);
    readonly Color C_BLUE    = Color.FromArgb(86,  156, 214);
    readonly Color C_GREEN   = Color.FromArgb(78,  201, 176);
    readonly Color C_ORANGE  = Color.FromArgb(220, 150, 80);
    readonly Color C_PURPLE  = Color.FromArgb(190, 140, 255);
    readonly Color C_WHITE   = Color.FromArgb(220, 220, 220);
    readonly Color C_GRAY    = Color.FromArgb(140, 140, 140);
    readonly Color C_DIM     = Color.FromArgb(75,  75,  75);

    // ── input boxes
    TextBox? aM, aShape, aScale, aNorm, aStd, aMin, aMax;
    TextBox? sM, sShape, sScale, sNorm, sStd, sMin, sMax;

    // ── result labels  (declared as fields so ShowResults can reach them)
    Label resLq   = new Label();
    Label resWq   = new Label();
    Label resW    = new Label();
    Label resL    = new Label();
    Label resRho  = new Label();
    Label resIdle = new Label();
    Label resModel= new Label();
    Label resInfo = new Label();

    // ── panels that get swapped
    Panel? arrFieldBox;
    Panel? svcFieldBox;
    Panel? arrPillRow;
    Panel? svcPillRow;
    Panel? resultSection;
    Label? errLabel;

    Button? bArrM, bArrG, bSvcM, bSvcG;
    Button[] arrPills = Array.Empty<Button>();
    Button[] svcPills = Array.Empty<Button>();

    public MainForm()
    {
        Text          = "Queuing Theory Calculator";
        Size          = new Size(1020, 860);
        MinimumSize   = new Size(900,  760);
        BackColor     = C_BG;
        ForeColor     = C_WHITE;
        Font          = new Font("Consolas", 9f);
        StartPosition = FormStartPosition.CenterScreen;
        BuildUI();
    }

    // ══════════════════════════════════════════════════════════════════
    //  TOP-LEVEL LAYOUT
    // ══════════════════════════════════════════════════════════════════
    void BuildUI()
    {
        // // header strip
        // var header       = new Panel { Dock = DockStyle.Top, Height = 56, BackColor = Color.FromArgb(35, 35, 38) };
        // header.Paint    += (s, e) => e.Graphics.DrawLine(new Pen(C_BORDER), 0, 55, header.Width, 55);
        // var hTitle       = Lbl("Queuing Theory Calculator", new Font("Segoe UI", 13f, FontStyle.Bold), C_WHITE);
        // hTitle.Location  = new Point(18, 8);
        // var hSub         = Lbl("Single Server  |  DCS-UOK", new Font("Consolas", 8f), C_GRAY);
        // hSub.Location    = new Point(19, 35);
        // header.Controls.Add(hTitle);
        // header.Controls.Add(hSub);
        // Controls.Add(header);

        // main tab control
        var tabs      = new TabControl { Dock = DockStyle.Fill };
        tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabs.Appearance = TabAppearance.Normal;
        tabs.ItemSize = new Size(130, 36);
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.Font     = new Font("Segoe UI", 10f, FontStyle.Bold);
        tabs.DrawItem += OnDrawMainTab;

        var tQueue = new TabPage { Text = "Queue",     BackColor = C_BG, Padding = new Padding(0) };
        var tSim   = new TabPage { Text = "Simulator", BackColor = C_BG };

        var simMsg = Lbl("Simulator — Coming Soon", new Font("Segoe UI", 13f), C_DIM);
        simMsg.TextAlign = ContentAlignment.MiddleCenter;
        simMsg.Dock      = DockStyle.Fill;
        tSim.Controls.Add(simMsg);

        tQueue.Controls.Add(BuildQueueTab());
        tabs.TabPages.Add(tQueue);
        tabs.TabPages.Add(tSim);
        Controls.Add(tabs);
    }

    void OnDrawMainTab(object? s, DrawItemEventArgs e)
    {
        var tc  = (TabControl)s!;
        bool on = e.Index == tc.SelectedIndex;
        e.Graphics.FillRectangle(new SolidBrush(on ? C_PANEL : Color.FromArgb(35,35,38)), e.Bounds);
        if (on) e.Graphics.DrawLine(new Pen(C_BLUE, 2), e.Bounds.Left, e.Bounds.Bottom-1, e.Bounds.Right, e.Bounds.Bottom-1);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(tc.TabPages[e.Index].Text, new Font("Segoe UI",10f,FontStyle.Bold),
            new SolidBrush(on ? C_BLUE : C_GRAY), e.Bounds, sf);
    }

    // ══════════════════════════════════════════════════════════════════
    //  QUEUE TAB  →  sub-tabs Mean / Rate
    // ══════════════════════════════════════════════════════════════════
    Control BuildQueueTab()
    {
        var sub       = new TabControl { Dock = DockStyle.Fill };
        sub.DrawMode  = TabDrawMode.OwnerDrawFixed;
        sub.Appearance= TabAppearance.FlatButtons;
        sub.ItemSize  = new Size(90, 28);
        sub.SizeMode  = TabSizeMode.Fixed;
        sub.Font      = new Font("Consolas", 9f, FontStyle.Bold);
        sub.DrawItem += OnDrawSubTab;

        var tMean = new TabPage { Text = "Mean", BackColor = C_BG, Padding = new Padding(0) };
        var tRate = new TabPage { Text = "Rate", BackColor = C_BG };

        var rateMsg = Lbl("Rate input — Coming Soon", new Font("Segoe UI",12f), C_DIM);
        rateMsg.TextAlign = ContentAlignment.MiddleCenter;
        rateMsg.Dock      = DockStyle.Fill;
        tRate.Controls.Add(rateMsg);

        tMean.Controls.Add(BuildMeanPanel());
        sub.TabPages.Add(tMean);
        sub.TabPages.Add(tRate);
        return sub;
    }

    void OnDrawSubTab(object? s, DrawItemEventArgs e)
    {
        var tc  = (TabControl)s!;
        bool on = e.Index == tc.SelectedIndex;
        e.Graphics.FillRectangle(new SolidBrush(on ? Color.FromArgb(52,52,55) : Color.FromArgb(35,35,38)), e.Bounds);
        if (on) e.Graphics.DrawLine(new Pen(C_GREEN,2), e.Bounds.Left, e.Bounds.Bottom-1, e.Bounds.Right, e.Bounds.Bottom-1);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(tc.TabPages[e.Index].Text, new Font("Consolas",9f,FontStyle.Bold),
            new SolidBrush(on ? C_GREEN : C_GRAY), e.Bounds, sf);
    }

    // ══════════════════════════════════════════════════════════════════
    //  MEAN PANEL  (scrollable)
    // ══════════════════════════════════════════════════════════════════
    Control BuildMeanPanel()
    {
        var scroll       = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = C_BG };
        int W            = 880;
        int y            = 18;

        // ── helper: add label at y, return new y
        void AddSec(string t)
        {
            var l = Lbl(t, new Font("Consolas",7.5f,FontStyle.Bold), C_DIM);
            l.Location = new Point(18, y);
            scroll.Controls.Add(l);
            y += 22;
        }

        // ── ARRIVAL DIST buttons ──────────────────────────────────────
        AddSec("ARRIVAL DISTRIBUTION");
        bArrM = DistBtn("M", "Exponential", true);
        bArrG = DistBtn("G", "General",    false);
        bArrM.Location = new Point(18, y);
        bArrG.Location = new Point(18 + 200, y);
        bArrM.Click   += (s,e) => SwitchArr(true);
        bArrG.Click   += (s,e) => SwitchArr(false);
        scroll.Controls.Add(bArrM);
        scroll.Controls.Add(bArrG);
        y += 74;

        // G pill row (arrival)
        arrPillRow          = new Panel { Location = new Point(18, y), Height = 34, Width = W, BackColor = C_BG, Visible = false };
        string[] pnames     = { "Gamma", "Normal", "Uniform" };
        arrPills            = new Button[3];
        int px              = 0;
        for (int i = 0; i < 3; i++)
        {
            string sub = pnames[i].ToLower();
            var pb     = PillBtn(pnames[i], sub == arrSub);
            pb.Location= new Point(px, 0);
            string cs  = sub;
            pb.Click  += (s,e) => SwitchArrSub(cs);
            arrPills[i]= pb;
            arrPillRow.Controls.Add(pb);
            px += pb.Width + 8;
        }
        scroll.Controls.Add(arrPillRow);
        y += 42;

        // ── SERVICE DIST buttons ──────────────────────────────────────
        AddSec("SERVICE DISTRIBUTION");
        bSvcM = DistBtn("M", "Exponential", true);
        bSvcG = DistBtn("G", "General",    false);
        bSvcM.Location = new Point(18, y);
        bSvcG.Location = new Point(18 + 200, y);
        bSvcM.Click   += (s,e) => SwitchSvc(true);
        bSvcG.Click   += (s,e) => SwitchSvc(false);
        scroll.Controls.Add(bSvcM);
        scroll.Controls.Add(bSvcG);
        y += 74;

        // G pill row (service)
        svcPillRow     = new Panel { Location = new Point(18, y), Height = 34, Width = W, BackColor = C_BG, Visible = false };
        svcPills       = new Button[3];
        px             = 0;
        for (int i = 0; i < 3; i++)
        {
            string sub = pnames[i].ToLower();
            var pb     = PillBtn(pnames[i], sub == svcSub);
            pb.Location= new Point(px, 0);
            string cs  = sub;
            pb.Click  += (s,e) => SwitchSvcSub(cs);
            svcPills[i]= pb;
            svcPillRow.Controls.Add(pb);
            px += pb.Width + 8;
        }
        scroll.Controls.Add(svcPillRow);
        y += 42;

        // ── INPUT PARAMETER CARDS ─────────────────────────────────────
        AddSec("INPUT PARAMETERS");

        // Arrival card
        var arrCard      = new Panel { Location = new Point(18, y), Width = 420, Height = 130, BackColor = C_PANEL };
        arrCard.Paint   += (s,e) => DrawCard(e.Graphics, arrCard, C_BLUE);
        var arrTitle     = Lbl("Inter-Arrival Time", new Font("Segoe UI",9.5f,FontStyle.Bold), C_WHITE);
        arrTitle.Location= new Point(12, 10);
        arrCard.Controls.Add(arrTitle);
        arrFieldBox      = new Panel { Location = new Point(12, 36), Width = 396, Height = 90, BackColor = C_PANEL };
        arrCard.Controls.Add(arrFieldBox);
        scroll.Controls.Add(arrCard);

        // Service card
        var svcCard      = new Panel { Location = new Point(18+420+18, y), Width = 420, Height = 130, BackColor = C_PANEL };
        svcCard.Paint   += (s,e) => DrawCard(e.Graphics, svcCard, C_GREEN);
        var svcTitle     = Lbl("Service Time", new Font("Segoe UI",9.5f,FontStyle.Bold), C_WHITE);
        svcTitle.Location= new Point(12, 10);
        svcCard.Controls.Add(svcTitle);
        svcFieldBox      = new Panel { Location = new Point(12, 36), Width = 396, Height = 90, BackColor = C_PANEL };
        svcCard.Controls.Add(svcFieldBox);
        scroll.Controls.Add(svcCard);

        RenderArrFields();
        RenderSvcFields();

        // keep card heights in sync when fields change
        void SyncCards()
        {
            int h = Math.Max(arrFieldBox.Bottom + 16, svcFieldBox.Bottom + 16);
            arrCard.Height = h;
            svcCard.Height = h;
        }

        y += 140;

        // ── COMPUTE BUTTON ────────────────────────────────────────────
        var btnC       = new Button { Text = "▶   COMPUTE", Location = new Point(18, y), Width = W, Height = 44 };
        btnC.FlatStyle = FlatStyle.Flat;
        btnC.BackColor = C_BLUE;
        btnC.ForeColor = Color.FromArgb(15, 15, 15);
        btnC.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
        btnC.Cursor    = Cursors.Hand;
        btnC.FlatAppearance.BorderSize = 0;
        btnC.Click    += BtnCompute_Click;
        scroll.Controls.Add(btnC);
        y += 52;

        // ── ERROR LABEL ───────────────────────────────────────────────
        errLabel          = new Label { Location = new Point(18, y), Width = W, Height = 0, Visible = false };
        errLabel.BackColor= Color.FromArgb(70,200,60,60);
        errLabel.ForeColor= Color.FromArgb(255,110,110);
        errLabel.Font     = new Font("Consolas", 8.5f);
        errLabel.Padding  = new Padding(10, 6, 0, 0);
        scroll.Controls.Add(errLabel);
        y += 0;

        // ── RESULTS SECTION ───────────────────────────────────────────
        resultSection          = BuildResultSection(W);
        resultSection.Location = new Point(18, y + 8);
        resultSection.Visible  = false;
        scroll.Controls.Add(resultSection);

        return scroll;
    }

    void DrawCard(Graphics g, Panel p, Color accent)
    {
        g.DrawRectangle(new Pen(C_BORDER), 0, 0, p.Width-1, p.Height-1);
        g.FillRectangle(new SolidBrush(accent), 0, 0, p.Width, 3);
    }

    // ══════════════════════════════════════════════════════════════════
    //  RESULTS SECTION  (6 cards in 2 rows × 3 cols)
    // ══════════════════════════════════════════════════════════════════
    Panel BuildResultSection(int W)
    {
        var sec       = new Panel { Width = W, Height = 300, BackColor = C_BG };

        // title row
        var titleLbl  = Lbl("RESULTS", new Font("Segoe UI",10f,FontStyle.Bold), C_GREEN);
        titleLbl.Location = new Point(0, 0);
        sec.Controls.Add(titleLbl);

        resModel.Font      = new Font("Consolas",8.5f);
        resModel.ForeColor = C_GREEN;
        resModel.BackColor = Color.FromArgb(22,78,201,176);
        resModel.AutoSize  = true;
        resModel.Padding   = new Padding(8,3,8,3);
        resModel.Location  = new Point(80, 2);
        sec.Controls.Add(resModel);

        // build 6 cards manually in 2 rows x 3 cols
        int cw = (W - 10) / 3;   // card width
        int ch = 110;              // card height
        int gap = 5;

        PlaceCard(sec, "Lq",   "Mean No. in Queue",   C_BLUE,   ref resLq,   0*cw + 0*gap, 28, cw, ch);
        PlaceCard(sec, "Wq",   "Mean Wait in Queue",  C_ORANGE, ref resWq,   1*cw + 1*gap, 28, cw, ch);
        PlaceCard(sec, "W",    "Mean Wait in System", C_WHITE,  ref resW,    2*cw + 2*gap, 28, cw, ch);
        PlaceCard(sec, "L",    "Mean No. in System",  C_GREEN,  ref resL,    0*cw + 0*gap, 28+ch+gap, cw, ch);
        PlaceCard(sec, "Rho",  "Traffic Intensity",   C_ORANGE, ref resRho,  1*cw + 1*gap, 28+ch+gap, cw, ch);
        PlaceCard(sec, "Idle", "Server Idle %",       C_GRAY,   ref resIdle, 2*cw + 2*gap, 28+ch+gap, cw, ch);

        sec.Height = 28 + ch*2 + gap*3 + 34;

        // info bar
        resInfo.Location  = new Point(0, 28 + ch*2 + gap*2 + 8);
        resInfo.Width     = W;
        resInfo.Height    = 26;
        resInfo.BackColor = Color.FromArgb(18, 86, 156, 214);
        resInfo.ForeColor = C_GRAY;
        resInfo.Font      = new Font("Consolas", 7.5f);
        resInfo.Padding   = new Padding(8, 4, 0, 0);
        sec.Controls.Add(resInfo);

        return sec;
    }

    void PlaceCard(Panel parent, string sym, string desc, Color col, ref Label outLbl, int x, int y, int w, int h)
    {
        var card       = new Panel { Location = new Point(x,y), Width = w, Height = h, BackColor = C_CARD };
        card.Paint    += (s,e) =>
        {
            e.Graphics.DrawRectangle(new Pen(C_BORDER), 0, 0, card.Width-1, card.Height-1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(100,col),2), 0, card.Height-1, card.Width, card.Height-1);
        };

        var symL       = Lbl(sym,  new Font("Consolas",8.5f,FontStyle.Italic), C_PURPLE);
        symL.Location  = new Point(10, 8);
        var descL      = Lbl(desc, new Font("Consolas",7.5f), C_GRAY);
        descL.Location = new Point(10, 26);

        var valL       = new Label { Text = "—", AutoSize = true, Location = new Point(10,44) };
        valL.ForeColor = col;
        valL.Font      = new Font("Segoe UI", 17f, FontStyle.Bold);
        outLbl         = valL;

        card.Controls.Add(symL);
        card.Controls.Add(descL);
        card.Controls.Add(valL);
        parent.Controls.Add(card);
    }

    // ══════════════════════════════════════════════════════════════════
    //  FIELD RENDERING
    // ══════════════════════════════════════════════════════════════════
    void RenderArrFields()
    {
        if (arrFieldBox == null) return;
        arrFieldBox.Controls.Clear();
        aM=aShape=aScale=aNorm=aStd=aMin=aMax=null;

        if (arrIsM)
        {
            aM = Field(arrFieldBox, "Mean Inter-Arrival Time (min)", "e.g. 10", 0);
            arrFieldBox.Height = 50;
        }
        else if (arrSub == "gamma")
        {
            aShape = Field(arrFieldBox, "Shape  (k)",   "e.g. 5", 0);
            aScale = Field(arrFieldBox, "Scale  (θ)",   "e.g. 2", 50);
            arrFieldBox.Height = 100;
        }
        else if (arrSub == "normal")
        {
            aNorm = Field(arrFieldBox, "Mean (min)",          "e.g. 10", 0);
            aStd  = Field(arrFieldBox, "Std Deviation (min)", "e.g. 4",  50);
            arrFieldBox.Height = 100;
        }
        else
        {
            aMin = Field(arrFieldBox, "Minimum (min)", "e.g. 7",  0);
            aMax = Field(arrFieldBox, "Maximum (min)", "e.g. 13", 50);
            arrFieldBox.Height = 100;
        }

        if (arrFieldBox.Parent != null)
        {
            arrFieldBox.Parent.Height = arrFieldBox.Bottom + 16;
            arrFieldBox.Parent.Invalidate();
        }
    }

    void RenderSvcFields()
    {
        if (svcFieldBox == null) return;
        svcFieldBox.Controls.Clear();
        sM=sShape=sScale=sNorm=sStd=sMin=sMax=null;

        if (svcIsM)
        {
            sM = Field(svcFieldBox, "Mean Service Time (min)", "e.g. 8", 0);
            svcFieldBox.Height = 50;
        }
        else if (svcSub == "gamma")
        {
            sShape = Field(svcFieldBox, "Shape  (k)", "e.g. 4", 0);
            sScale = Field(svcFieldBox, "Scale  (θ)", "e.g. 2", 50);
            svcFieldBox.Height = 100;
        }
        else if (svcSub == "normal")
        {
            sNorm = Field(svcFieldBox, "Mean (min)",          "e.g. 8", 0);
            sStd  = Field(svcFieldBox, "Std Deviation (min)", "e.g. 3", 50);
            svcFieldBox.Height = 100;
        }
        else
        {
            sMin = Field(svcFieldBox, "Minimum (min)", "e.g. 7", 0);
            sMax = Field(svcFieldBox, "Maximum (min)", "e.g. 9", 50);
            svcFieldBox.Height = 100;
        }

        if (svcFieldBox.Parent != null)
        {
            svcFieldBox.Parent.Height = svcFieldBox.Bottom + 16;
            svcFieldBox.Parent.Invalidate();
        }
    }

    TextBox Field(Panel parent, string lbl, string hint, int top)
    {
        var l      = Lbl(lbl, new Font("Consolas",7.5f), C_GRAY);
        l.Location = new Point(0, top);
        parent.Controls.Add(l);

        var tb            = new TextBox { Width = 370, Location = new Point(0, top+18) };
        tb.BackColor      = C_INPUT;
        tb.ForeColor      = C_WHITE;
        tb.BorderStyle    = BorderStyle.FixedSingle;
        tb.Font           = new Font("Consolas", 9.5f);
        tb.PlaceholderText= hint;
        tb.TextChanged   += (s,e) => HideResults();
        parent.Controls.Add(tb);
        return tb;
    }

    // ══════════════════════════════════════════════════════════════════
    //  DIST BUTTONS  &  PILLS
    // ══════════════════════════════════════════════════════════════════
    Button DistBtn(string letter, string desc, bool active)
    {
        var b      = new Button { Width = 190, Height = 64, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        SetDistStyle(b, active);
        string L   = letter;
        string D   = desc;
        b.Paint   += (s,e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.DrawString(L, new Font("Segoe UI",18f,FontStyle.Bold), new SolidBrush(b.ForeColor), 12, 5);
            e.Graphics.DrawString(D, new Font("Consolas",7.5f), new SolidBrush(C_GRAY), 12, 40);
        };
        return b;
    }

    void SetDistStyle(Button b, bool active)
    {
        b.BackColor                 = active ? Color.FromArgb(25,86,156,214) : C_PANEL;
        b.ForeColor                 = active ? C_BLUE : C_GRAY;
        b.FlatAppearance.BorderColor= active ? C_BLUE : C_BORDER;
        b.FlatAppearance.BorderSize = 1;
        b.Invalidate();
    }

    Button PillBtn(string text, bool active)
    {
        var b      = new Button { Text = text, AutoSize = true, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        b.Font     = new Font("Consolas", 8.5f);
        b.Padding  = new Padding(12, 4, 12, 4);
        SetPillStyle(b, active);
        return b;
    }

    void SetPillStyle(Button b, bool active)
    {
        b.BackColor                 = active ? Color.FromArgb(22,78,201,176) : C_PANEL;
        b.ForeColor                 = active ? C_GREEN : C_GRAY;
        b.FlatAppearance.BorderColor= active ? C_GREEN : C_BORDER;
        b.FlatAppearance.BorderSize = 1;
    }

    // ══════════════════════════════════════════════════════════════════
    //  STATE SWITCHES
    // ══════════════════════════════════════════════════════════════════
    void SwitchArr(bool isM)
    {
        arrIsM = isM;
        SetDistStyle(bArrM!, isM);
        SetDistStyle(bArrG!, !isM);
        arrPillRow!.Visible = !isM;
        RenderArrFields();
        HideResults();
    }

    void SwitchSvc(bool isM)
    {
        svcIsM = isM;
        SetDistStyle(bSvcM!, isM);
        SetDistStyle(bSvcG!, !isM);
        svcPillRow!.Visible = !isM;
        RenderSvcFields();
        HideResults();
    }

    void SwitchArrSub(string sub)
    {
        arrSub = sub;
        string[] s = { "gamma","normal","uniform" };
        for (int i = 0; i < arrPills.Length; i++) SetPillStyle(arrPills[i], s[i]==sub);
        RenderArrFields();
        HideResults();
    }

    void SwitchSvcSub(string sub)
    {
        svcSub = sub;
        string[] s = { "gamma","normal","uniform" };
        for (int i = 0; i < svcPills.Length; i++) SetPillStyle(svcPills[i], s[i]==sub);
        RenderSvcFields();
        HideResults();
    }

    // ══════════════════════════════════════════════════════════════════
    //  COMPUTE
    // ══════════════════════════════════════════════════════════════════
    void BtnCompute_Click(object? sender, EventArgs e)
    {
        HideResults();

        double arrMean, arrVar;

        if (arrIsM)
        {
            if (!Pos(aM, "Arrival mean", out arrMean)) return;
            arrVar = QueueEngine.GetExponentialVariance(arrMean);
        }
        else if (arrSub == "gamma")
        {
            if (!Pos(aShape, "Arrival shape",    out double shape)) return;
            if (!Pos(aScale, "Arrival scale",    out double scale)) return;
            arrMean = QueueEngine.GetGammaMean(shape, scale);
            arrVar  = QueueEngine.GetGammaVariance(shape, scale);
        }
        else if (arrSub == "normal")
        {
            if (!Pos(aNorm, "Arrival mean",      out arrMean))     return;
            if (!Pos(aStd,  "Arrival std dev",   out double std))  return;
            arrVar = QueueEngine.GetNormalVariance(std);
        }
        else
        {
            if (!Pos(aMin, "Arrival min", out double mn)) return;
            if (!Pos(aMax, "Arrival max", out double mx)) return;
            if (mn >= mx) { Err("Arrival min must be less than max."); return; }
            arrMean = QueueEngine.GetUniformMean(mn, mx);
            arrVar  = QueueEngine.GetUniformVariance(mn, mx);
        }

        double svcMean, svcVar;

        if (svcIsM)
        {
            if (!Pos(sM, "Service mean", out svcMean)) return;
            svcVar = QueueEngine.GetExponentialVariance(svcMean);
        }
        else if (svcSub == "gamma")
        {
            if (!Pos(sShape, "Service shape",    out double shape)) return;
            if (!Pos(sScale, "Service scale",    out double scale)) return;
            svcMean = QueueEngine.GetGammaMean(shape, scale);
            svcVar  = QueueEngine.GetGammaVariance(shape, scale);
        }
        else if (svcSub == "normal")
        {
            if (!Pos(sNorm, "Service mean",      out svcMean))    return;
            if (!Pos(sStd,  "Service std dev",   out double std)) return;
            svcVar = QueueEngine.GetNormalVariance(std);
        }
        else
        {
            if (!Pos(sMin, "Service min", out double mn)) return;
            if (!Pos(sMax, "Service max", out double mx)) return;
            if (mn >= mx) { Err("Service min must be less than max."); return; }
            svcMean = QueueEngine.GetUniformMean(mn, mx);
            svcVar  = QueueEngine.GetUniformVariance(mn, mx);
        }

        try
        {
            var r = QueueEngine.Compute(arrMean, arrVar, svcMean, svcVar, arrIsM, svcIsM);
            ShowResults(r);
        }
        catch (Exception ex) { Err(ex.Message); }
    }

    bool Pos(TextBox? tb, string name, out double val)
    {
        val = 0;
        if (tb == null || !double.TryParse(tb.Text, out val) || val <= 0)
        { Err(name + ": enter a positive number."); return false; }
        return true;
    }

    void ShowResults(QueueResults r)
    {
        resModel.Text = r.ModelName;
        resLq.Text    = r.Lq.ToString("F4");
        resWq.Text    = r.Wq.ToString("F4") + " min";
        resW.Text     = r.W.ToString("F4")  + " min";
        resL.Text     = r.L.ToString("F4");
        resRho.Text   = r.Rho.ToString("F4");
        resIdle.Text  = (r.IdleProp * 100).ToString("F2") + "%";
        resInfo.Text  = "  λ=" + r.Lambda.ToString("F4") + "  μ=" + r.Mu.ToString("F4") + "  ρ=" + r.Rho.ToString("F4") + "  model=" + r.ModelName;

        if (errLabel != null) { errLabel.Visible = false; errLabel.Height = 0; }

        // reposition result section just below the error label
        if (errLabel != null && resultSection != null)
            resultSection.Location = new Point(18, errLabel.Top + 8);

        resultSection!.Visible = true;
    }

    void HideResults()
    {
        if (resultSection != null) resultSection.Visible = false;
    }

    void Err(string msg)
    {
        if (errLabel == null) return;
        errLabel.Text    = "  ⚠  " + msg;
        errLabel.Height  = 40;
        errLabel.Visible = true;
    }

    // ══════════════════════════════════════════════════════════════════
    //  HELPERS
    // ══════════════════════════════════════════════════════════════════
    static Label Lbl(string text, Font font, Color color)
    {
        return new Label { Text = text, Font = font, ForeColor = color, AutoSize = true };
    }
}