using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AForge;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using KeyEventArgs=System.Windows.Input.KeyEventArgs;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point=System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace bbplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Random random = new Random();

        private readonly Board board;

        //private Point boardTopLeft;

        public MainWindow()
        {
            InitializeComponent();

//            var timer = new DispatcherTimer();
//            timer.Interval = TimeSpan.FromMilliseconds( 100 );
//            timer.Tick += CapturePixelColorUnderCursor;
//            timer.Start();

            this.board = new Board();
            this.InitializeBoard();
        }


        private void InitializeBoard()
        {
            board[0, 0] = new BoardPosition(b11);
            board[0, 1] = new BoardPosition(b12);
            board[0, 2] = new BoardPosition(b13);
            board[0, 3] = new BoardPosition(b14);
            board[0, 4] = new BoardPosition(b15);
            board[0, 5] = new BoardPosition(b16);
            board[0, 6] = new BoardPosition(b17);
            board[0, 7] = new BoardPosition(b18);

            board[1, 0] = new BoardPosition(b21);
            board[1, 1] = new BoardPosition(b22);
            board[1, 2] = new BoardPosition(b23);
            board[1, 3] = new BoardPosition(b24);
            board[1, 4] = new BoardPosition(b25);
            board[1, 5] = new BoardPosition(b26);
            board[1, 6] = new BoardPosition(b27);
            board[1, 7] = new BoardPosition(b28);

            board[2, 0] = new BoardPosition(b31);
            board[2, 1] = new BoardPosition(b32);
            board[2, 2] = new BoardPosition(b33);
            board[2, 3] = new BoardPosition(b34);
            board[2, 4] = new BoardPosition(b35);
            board[2, 5] = new BoardPosition(b36);
            board[2, 6] = new BoardPosition(b37);
            board[2, 7] = new BoardPosition(b38);

            board[3, 0] = new BoardPosition(b41);
            board[3, 1] = new BoardPosition(b42);
            board[3, 2] = new BoardPosition(b43);
            board[3, 3] = new BoardPosition(b44);
            board[3, 4] = new BoardPosition(b45);
            board[3, 5] = new BoardPosition(b46);
            board[3, 6] = new BoardPosition(b47);
            board[3, 7] = new BoardPosition(b48);

            board[4, 0] = new BoardPosition(b51);
            board[4, 1] = new BoardPosition(b52);
            board[4, 2] = new BoardPosition(b53);
            board[4, 3] = new BoardPosition(b54);
            board[4, 4] = new BoardPosition(b55);
            board[4, 5] = new BoardPosition(b56);
            board[4, 6] = new BoardPosition(b57);
            board[4, 7] = new BoardPosition(b58);

            board[5, 0] = new BoardPosition(b61);
            board[5, 1] = new BoardPosition(b62);
            board[5, 2] = new BoardPosition(b63);
            board[5, 3] = new BoardPosition(b64);
            board[5, 4] = new BoardPosition(b65);
            board[5, 5] = new BoardPosition(b66);
            board[5, 6] = new BoardPosition(b67);
            board[5, 7] = new BoardPosition(b68);

            board[6, 0] = new BoardPosition(b71);
            board[6, 1] = new BoardPosition(b72);
            board[6, 2] = new BoardPosition(b73);
            board[6, 3] = new BoardPosition(b74);
            board[6, 4] = new BoardPosition(b75);
            board[6, 5] = new BoardPosition(b76);
            board[6, 6] = new BoardPosition(b77);
            board[6, 7] = new BoardPosition(b78);

            board[7, 0] = new BoardPosition(b81);
            board[7, 1] = new BoardPosition(b82);
            board[7, 2] = new BoardPosition(b83);
            board[7, 3] = new BoardPosition(b84);
            board[7, 4] = new BoardPosition(b85);
            board[7, 5] = new BoardPosition(b86);
            board[7, 6] = new BoardPosition(b87);
            board[7, 7] = new BoardPosition(b88);
        }

//        private byte[] ImageToByteArray(Image image)
//        {
//            var ms = new MemoryStream();
//            image.Save(ms, image.RawFormat);
//            return ms.ToArray();
//        }
//
//        private BitmapImage ImageFromBuffer(byte[] bytes)
//        {
//            var stream = new MemoryStream(bytes);
//            var bm = new BitmapImage();
//            bm.BeginInit();
//            bm.StreamSource = stream;
//            bm.EndInit();
//            return bm;
//        }

        private Point CalibrateBoardLocation(Point cursorLocation)
        {
            var darkGrayBackground = Color.FromRgb(11, 11, 11);

            int topLeftX = cursorLocation.X;
            int topLeftY = cursorLocation.Y;

            Action<int, int> onEachIteration = (x, y) => SetCursorPos(x, y);

            var leftColor = GetPixelColorAt(GetDC(IntPtr.Zero), --topLeftX, topLeftY);
            while (!leftColor.Equals(darkGrayBackground))
            {
                if (Math.Abs(cursorLocation.X - topLeftX) > BoardPiece.Width)
                    break;

                leftColor = GetPixelColorAt(GetDC(IntPtr.Zero), --topLeftX, topLeftY);
                onEachIteration(topLeftX, topLeftY);
            }
            while (leftColor.Equals(darkGrayBackground))
            {
                if (Math.Abs(cursorLocation.X - topLeftX) > BoardPiece.Width)
                    break;

                leftColor = GetPixelColorAt(GetDC(IntPtr.Zero), --topLeftX, topLeftY);
                onEachIteration(topLeftX, topLeftY);
            }
            topLeftX++;

            var topColor = GetPixelColorAt(GetDC(IntPtr.Zero), topLeftX, --topLeftY);
            while (!topColor.Equals(darkGrayBackground))
            {
                if (Math.Abs(cursorLocation.Y - topLeftY) > BoardPiece.Height)
                    break;

                topColor = GetPixelColorAt(GetDC(IntPtr.Zero), topLeftX, --topLeftY);
                onEachIteration(topLeftX, topLeftY);
            }
            while (topColor.Equals(darkGrayBackground))
            {
                if (Math.Abs(cursorLocation.Y - topLeftY) > BoardPiece.Height)
                    break;

                topColor = GetPixelColorAt(GetDC(IntPtr.Zero), topLeftX, --topLeftY);
                onEachIteration(topLeftX, topLeftY);
            }

            return new Point(topLeftX, topLeftY);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.Key == Key.D1 )
            {
                Point cursor;
                GetCursorPos( out cursor );

                var calibratedBoardTopLeft = this.CalibrateBoardLocation(cursor);

                this.board.ScreenTopLeft = calibratedBoardTopLeft;
                this.UpdateBoard();

                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds( 100 );
                timer.Tick += delegate
                {
                    this.UpdateBoard();

                    var solution = this.FindSolution();
                    this.HighlightSolution(solution);

                };
                timer.Start();
            }

            if ( e.Key == Key.D2 )
            {
                this.UpdateBoard();
            }

            if ( e.Key == Key.D3 )
            {
                var solution = this.FindSolution(chooseRandom: false);
                this.ApplySolution(solution);
            }

            if ( e.Key == Key.D4 )
            {
                this.UpdateBoard();

                int tryCounter = 0;
                while (this.unknownCount > 0)
                {
                    this.UpdateBoard();
                    Thread.Sleep( 100 );

                    if ((tryCounter++) > 10)
                        break;
                }

                Thread.Sleep( 100 );
                var solution = this.FindSolution(chooseRandom: false);
                this.ApplySolution(solution);

                Thread.Sleep( 250 );
                this.UpdateBoard();
            }
            
            if ( e.Key == Key.D5 )
            {
                this.UpdateBoard();

                int tryCounter = 0;
                while (this.unknownCount > 0)
                {
                    this.UpdateBoard();
                    Thread.Sleep( 100 );

                    if ((tryCounter++) > 10)
                        break;
                }

                Thread.Sleep( 100 );
                var solution = this.FindSolution(chooseRandom: true);
                this.ApplySolution(solution);

                Thread.Sleep( 250 );
                this.UpdateBoard();
            }

            if ( e.Key == Key.H )
            {
                //UseHypercube();
            }
          

            base.OnKeyDown(e);
        }

        private void CleanBitmap(Bitmap bitmap)
        {
            var darkGrayBackground = System.Drawing.Color.FromArgb(11, 11, 11);
            var lightGrayBackground = System.Drawing.Color.FromArgb(47, 47, 47);

            var artifactColor1 = System.Drawing.Color.FromArgb(13, 13, 13);
            var artifactColor2 = System.Drawing.Color.FromArgb(45, 45, 45);
            var artifactColor3 = System.Drawing.Color.FromArgb(43, 43, 43);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.Equals(lightGrayBackground)
                        || pixel.Equals(artifactColor1)
                        || pixel.Equals(artifactColor2)
                        || pixel.Equals(artifactColor3))
                    {
                        bitmap.SetPixel(x, y, darkGrayBackground);
                    }
                }                
            }
        }

        private Bitmap UpdateBoardImage(Point topLeft)
        {
            var bottomRight = new Point(
                topLeft.X + (BoardPiece.Width*8), 
                topLeft.Y + (BoardPiece.Height*8));

            var size = new System.Drawing.Size(
                bottomRight.X - topLeft.X, 
                bottomRight.Y - topLeft.Y);

            var bitmap = new Bitmap(size.Width, size.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(topLeft, new Point(0, 0), size);

           CleanBitmap(bitmap);

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(), 
                IntPtr.Zero,
                Int32Rect.Empty, 
                BitmapSizeOptions.FromEmptyOptions());

            imgBoardBitmap.Source = bitmapSource;
            return bitmap;
        }

        private void UpdateBoard()
        {
            var bitmap = this.UpdateBoardImage(this.board.ScreenTopLeft);
            this.unknownCount = this.board.UpdateBoardImage(bitmap);
        }

        private Solution FindSolution(bool chooseRandom = false)
        {
            var solutions = this.board.FindSolutions();

            if (solutions == null)
            {
                this.solutionDescription.Content = "No solution found";
                return null;
            }

            Solution solution;

            if (chooseRandom)
            {
                int randomSolution = random.Next(0, solutions.Length - 1);
                solution = solutions[randomSolution];
            }
            else
            {
                solution = solutions.First();
            }

            this.solutionDescription.Content = string.Format("Solution: (weight:{4}) x:{0}, y:{1} to x{2}, y:{3}",
                solution.ArrayPosition1.X,
                solution.ArrayPosition1.Y,
                solution.ArrayPosition2.X,
                solution.ArrayPosition2.Y,
                solution.Weight);

            return solution;
        }

        private void HighlightSolution(Solution sol)
        {
            rectSolutionSource.Visibility = Visibility.Visible;
            rectSolutionDest.Visibility = Visibility.Visible;

            var sourceTopLeftY = Canvas.GetTop(this.imgBoardBitmap) + (sol.ArrayPosition1.Y*BoardPiece.Height);
            var sourceTopLeftX = Canvas.GetLeft(this.imgBoardBitmap) + (sol.ArrayPosition1.X*BoardPiece.Width);
            
            var destTopLeftY = Canvas.GetTop(this.imgBoardBitmap) + (sol.ArrayPosition2.Y*BoardPiece.Height);
            var destTopLeftX = Canvas.GetLeft(this.imgBoardBitmap) + (sol.ArrayPosition2.X*BoardPiece.Width);

            Canvas.SetTop(rectSolutionSource, sourceTopLeftY);
            Canvas.SetLeft(rectSolutionSource, sourceTopLeftX);

            Canvas.SetTop(rectSolutionDest, destTopLeftY);
            Canvas.SetLeft(rectSolutionDest, destTopLeftX);
        }

        private void ApplySolution( Solution sol )
        {
            SetCursorPos(
                this.board.ScreenTopLeft.X + 20 + (sol.ArrayPosition1.X*BoardPiece.Width),
                this.board.ScreenTopLeft.Y + 20 + (sol.ArrayPosition1.Y*BoardPiece.Height));

            mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
            Thread.Sleep( 50 );
            mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );

            Thread.Sleep( 100 );

            SetCursorPos(
                this.board.ScreenTopLeft.X + 20 + (sol.ArrayPosition2.X*BoardPiece.Width),
                this.board.ScreenTopLeft.Y + 20 + (sol.ArrayPosition2.Y*BoardPiece.Height));

            mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
            Thread.Sleep( 50 );
            mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );

            SetForegroundWindow(new WindowInteropHelper(this).Handle);       
        }

        private int unknownCount;

//        private void UseHypercube()
//        {
//            var ev = FindHypercube();
//            if ( ev.HyperCubePosition == null )
//            {
//                return;
//            }
//
//            var top = _board.TopOf( ev.HyperCubePosition );
//            var right = _board.RightOf( ev.HyperCubePosition );
//            var bottom = _board.BottomOf( ev.HyperCubePosition );
//            var left = _board.LeftOf( ev.HyperCubePosition );
//
//            Tuple<Surround,int>[] weights = new[]
//            {
//                new Tuple<Surround, int>( Surround.Top, ( top == null ) ? 0 : ev.RootBoardPieceWeights[top.Piece.RootBoardPiece] ),
//                new Tuple<Surround, int>( Surround.Right, ( right == null ) ? 0 : ev.RootBoardPieceWeights[right.Piece.RootBoardPiece] ),
//                new Tuple<Surround, int>( Surround.Bottom, ( bottom == null ) ? 0 : ev.RootBoardPieceWeights[bottom.Piece.RootBoardPiece] ),
//                new Tuple<Surround, int>( Surround.Left, ( left == null ) ? 0 : ev.RootBoardPieceWeights[left.Piece.RootBoardPiece] ),
//            };
//
//            var moveHyperCube = weights.OrderByDescending( m => m.Second ).Select( m => m.First ).First();
//
//            Point moveTo = new Point();
//            switch ( moveHyperCube )
//            {
//                case Surround.Top:
//                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X, ev.HyperCubePosition.ArrayPosition.Y - 1 );
//                    break;
//
//                case Surround.Right:
//                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X + 1, ev.HyperCubePosition.ArrayPosition.Y );
//                    break;
//
//                case Surround.Bottom:
//                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X, ev.HyperCubePosition.ArrayPosition.Y + 1 );
//                    break;
//
//                case Surround.Left:
//                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X - 1, ev.HyperCubePosition.ArrayPosition.Y );
//                    break;
//            }
//
//            var solution = new Solution( ev.HyperCubePosition.ArrayPosition, moveTo );
//            ApplySolution( solution );
//        }

        public enum Surround
        {
            Top,
            Right,
            Bottom,
            Left
        }


//        private HyperCubeEvaluation FindHypercube()
//        {
//            HyperCubeEvaluation output = new HyperCubeEvaluation();
//
//            for ( int y = 0; y < 8; y++ )
//            {
//                for ( int x = 0; x < 8; x++ )
//                {
//                    int setX = _boardCalibration.X + ( 40 * x );
//                    int setY = _boardCalibration.Y + ( 40 * y );
//
//                    var dc = CreateDC( "Display", null, null, IntPtr.Zero );
//
//                    var color = GetColorPointsFrom( dc, setX, setY );
//
//                    DeleteDC( dc );
//
//                    var boardPiece = BoardPiece.FindMatch( color );
//                    
//                    if ( boardPiece == BoardPiece.HyperCube )
//                    {
//                        output.HyperCubePosition = new BoardPosition();
//                        output.HyperCubePosition.SetPiece( boardPiece, new Point( setX, setY ), y, x );
//                    }
//
//                    if ( ! output.RootBoardPieceWeights.ContainsKey( boardPiece.RootBoardPiece ) )
//                    {
//                        output.RootBoardPieceWeights[boardPiece.RootBoardPiece] = boardPiece.Weight;
//                    }
//                    else
//                    {
//                        output.RootBoardPieceWeights[boardPiece.RootBoardPiece] += boardPiece.Weight;
//                    }
//                }
//            }
//
//            return output;
//        }

        class HyperCubeEvaluation
        {
            public HyperCubeEvaluation()
            {
                HyperCubePosition = null;
                RootBoardPieceWeights = new Dictionary<RootBoardPiece, int>();
            }

            public BoardPosition HyperCubePosition { get; set; }
            public IDictionary<RootBoardPiece, int> RootBoardPieceWeights { get; private set; }
        }




        private Color GetPixelColorAt( IntPtr dc, int x, int y )
        {
            var pixel = GetPixel( dc , x, y );
            var color = ColorTranslator.FromWin32( pixel );

            return Color.FromArgb( color.A, color.R, color.G, color.B );
        }

        private void CapturePixelColorUnderCursor( object sender, EventArgs e )
        {
            Point cursorPos;
            GetCursorPos( out cursorPos );

            var color = GetPixelColorAt( GetDC( IntPtr.Zero ), cursorPos.X, cursorPos.Y );

            var c2 = System.Drawing.Color.FromArgb( color.A, color.R, color.G, color.B );
         

            bgColorStatus.Text =
                string.Format( "H:{0}; S:{1}; B:{2} | ", c2.GetHue(), c2.GetSaturation(), c2.GetBrightness() )
                + c2.ToString();
            cursorXStatus.Text = "X: " + cursorPos.X;
            cursorYStatus.Text = "Y: " + cursorPos.Y;

            var bgColor = System.Windows.Media.Color.FromArgb( c2.A, c2.R, c2.G, c2.B );
            coloredStatus.Background = new SolidColorBrush( bgColor );
            coloredStatus.Width = 50;
        }
        
        
        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool GetCursorPos( out Point lpPoint );

        [DllImport( "user32.dll" )]
        private static extern bool SetCursorPos( int x, int y );

        [DllImport( "Gdi32.dll" )]
        private static extern int GetPixel( IntPtr hdc, int nXPos, int nYPos );

        [DllImport( "gdi32.dll" )]
        private static extern IntPtr CreateDC( string strDriver, string strDevice, string strOutput, IntPtr pData );

        [DllImport( "User32.dll" )]
        private static extern IntPtr GetDC( IntPtr wnd );

        [DllImport("user32.dll")]
        static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

        [DllImport( "User32.dll" )]
        private static extern void ReleaseDC( IntPtr dc );

        [DllImport("gdi32.dll")]
        static extern bool DeleteDC(IntPtr hdc);

        [DllImport( "User32.dll" )]
        private static extern IntPtr SetCapture( IntPtr hWnd );

        [DllImport( "User32.dll" )]
        private static extern int ReleaseCapture( IntPtr hWnd );

        [DllImport("user32.dll")]
        static extern void mouse_event( uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo );
        
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        /// <summary>Values to pass to the GetDCEx method.</summary>
        [Flags()]
        protected enum DeviceContextValues : uint
        {
            /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather 
            /// than the client rectangle.</summary>
            Window       = 0x00000001,
            /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC 
            /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
            Cache        = 0x00000002,
            /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the 
            /// default attributes when this DC is released.</summary>
            NoResetAttrs     = 0x00000004,
            /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows 
            /// below the window identified by hWnd.</summary>
            ClipChildren     = 0x00000008,
            /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows 
            /// above the window identified by hWnd.</summary>
            ClipSiblings     = 0x00000010,
            /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The 
            /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is 
            /// set to the upper-left corner of the window identified by hWnd.</summary>
            ParentClip       = 0x00000020,
            /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded 
            /// from the visible region of the returned DC.</summary>
            ExcludeRgn       = 0x00000040,
            /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is 
            /// intersected with the visible region of the returned DC.</summary>
            IntersectRgn     = 0x00000080,
            /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
            ExcludeUpdate    = 0x00000100,
            /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
            IntersectUpdate  = 0x00000200,
            /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate 
            /// call in effect that would otherwise exclude this window. Used for drawing during 
            /// tracking.</summary>
            LockWindowUpdate = 0x00000400,
            /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to 
            /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and 
            /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
            Validate     = 0x00200000,
        }

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool RegisterHotKey( IntPtr hWnd, int id, uint fsModifiers, uint vk );

        private Point highlightedPiece;

        private void RectangleOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var rectangle = (System.Windows.Shapes.Rectangle) sender;
            var name = rectangle.Name;
            var rectangleX = Convert.ToInt32(name.Substring(2, 1)) - 1;
            var rectangleY = Convert.ToInt32(name.Substring(1, 1)) - 1;

            this.highlightedPiece = new Point(rectangleX, rectangleY);

            imageHighlight.Stroke = Brushes.Red;
            imageHighlight.StrokeThickness = 3;
            imageHighlight.Fill = Brushes.Transparent;
            imageHighlight.Visibility = Visibility.Visible;

            var bitmapTopLeftY = Canvas.GetTop(imgBoardBitmap) + (rectangleY*BoardPiece.Height);
            var bitmapTopLeftX = Canvas.GetLeft(this.imgBoardBitmap) + (rectangleX*BoardPiece.Width);

            Canvas.SetTop(imageHighlight, bitmapTopLeftY);
            Canvas.SetLeft(imageHighlight, bitmapTopLeftX);

            var matches = BoardPiece.FindMatches(this.board.BoardImage, rectangleX, rectangleY);

            var closestMatch = matches.GetClosestMatch();

            listBox1.Items.Clear();

            foreach (var match in matches)
            {
                bool isClosest = closestMatch.BoardPiece.Name == match.BoardPiece.Name;
                var listBoxMatch = new ListBoxMatch(match, isClosest);
                
                listBox1.Items.Add(listBoxMatch);

                if (isClosest)
                    listBox1.SelectedItem = listBoxMatch;
            }

            this.ListBox1_OnPreviewMouseUp(listBox1, null);
        }

        private void ListBox1_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mp = (System.Windows.Controls.ListBox) sender;
            var item = (ListBoxMatch) mp.SelectedItem;
            if (item == null || item.MatchPair.BoardPiece.Name == "Unknown")
                return;

            rectSourcePiece.Fill = new ImageBrush(ImageUtility.BitmapToImageSource(item.MatchPair.BoardPiece.GetImage(), ImageFormat.Bmp));

            var piecePoint = new Point(this.highlightedPiece.X*BoardPiece.Width, this.highlightedPiece.Y*BoardPiece.Height);
            var boardPiece = this.board.BoardImage.Clone(
                new Rectangle(piecePoint, new System.Drawing.Size(BoardPiece.Width, BoardPiece.Height)),
                PixelFormat.DontCare);

            rectBoardPiece.Fill = new ImageBrush(ImageUtility.BitmapToImageSource(boardPiece, ImageFormat.Bmp));

            lblColorDist.Content = item.MatchPair.Weight.ColorDistance.ToString("##.####");
            lblLuminanceDist.Content = item.MatchPair.Weight.LuminanceDistance.ToString("##.####");

            rectBoardPieceAverageColor.Fill = new SolidColorBrush(ColorUtility.ConvertToMediaColor(ColorUtility.GetAveragePieceColor(boardPiece, 0, 0)));
            rectBoardPieceAverageLuminance.Fill = new SolidColorBrush(ColorUtility.ConvertToMediaColor(ColorUtility.GetAveragePieceLuminance(boardPiece, 0, 0)));

            rectSourcePieceAverageColor.Fill = new SolidColorBrush(ColorUtility.ConvertToMediaColor(ColorUtility.GetAveragePieceColor(item.MatchPair.BoardPiece.GetImage(), 0, 0)));
            rectSourcePieceAverageLuminance.Fill = new SolidColorBrush(ColorUtility.ConvertToMediaColor(ColorUtility.GetAveragePieceLuminance(item.MatchPair.BoardPiece.GetImage(), 0, 0)));

            // histograms
            var sourceHistogram = ColorUtility.GetLuminanceHistogram(item.MatchPair.BoardPiece.GetImage(), 0, 0);
            polySourceHistogram.Points = this.GetHistogramPointCollection(sourceHistogram);

            var pieceHistogram = ColorUtility.GetLuminanceHistogram(boardPiece, 0, 0);
            polyPieceHistogram.Points = this.GetHistogramPointCollection(pieceHistogram);

            var histogramDistance = ColorUtility.GetHistogramDistance(sourceHistogram, pieceHistogram);
            lblHistogramDist.Content = histogramDistance.ToString("##.####");
        }

        private PointCollection GetHistogramPointCollection(int[] histogram)
        {
            int max = histogram.Max();
            var points = new PointCollection();
            points.Add(new System.Windows.Point(0, max));
            for (int i = 0; i < histogram.Length; i++)
            {
                points.Add(new System.Windows.Point(i, max - histogram[i]));
            }
            points.Add(new System.Windows.Point(histogram.Length - 1, max));

            return points;
        }
    }

    class ListBoxMatch
    {
        private BoardPiece.MatchPair pair;
        private readonly bool isClosest;

        public ListBoxMatch(BoardPiece.MatchPair pair, bool isClosest)
        {
            this.pair = pair;
            this.isClosest = isClosest;
        }

        public string IsClosest
        {
            get { return (this.isClosest) ? "*" : ""; }
        }

        public BoardPiece.MatchPair MatchPair
        {
            get { return this.pair; }
        }
    }
}
