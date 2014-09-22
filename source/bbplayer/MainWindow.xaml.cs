using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Brush=System.Drawing.Brush;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using KeyEventArgs=System.Windows.Input.KeyEventArgs;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point=System.Drawing.Point;

namespace bbplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _mainWindow;
        private Point _boardCalibration;
        private Board _board;
        private IntPtr _dc;

        private Point _boardTopLeft, _boardBottomRight;

        public MainWindow()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds( 100 );
            timer.Tick += CapturePixelColorUnderCursor;
            timer.Start();

            _board = new Board();
            InitializeBoard();

        }


        private void InitializeBoard()
        {
            _board[0, 0] = new BoardPosition( b11 );
            _board[0, 1] = new BoardPosition( b12 );
            _board[0, 2] = new BoardPosition( b13 );
            _board[0, 3] = new BoardPosition( b14 );
            _board[0, 4] = new BoardPosition( b15 );
            _board[0, 5] = new BoardPosition( b16 );
            _board[0, 6] = new BoardPosition( b17 );
            _board[0, 7] = new BoardPosition( b18 );

            _board[1, 0] = new BoardPosition( b21 );
            _board[1, 1] = new BoardPosition( b22 );
            _board[1, 2] = new BoardPosition( b23 );
            _board[1, 3] = new BoardPosition( b24 );
            _board[1, 4] = new BoardPosition( b25 );
            _board[1, 5] = new BoardPosition( b26 );
            _board[1, 6] = new BoardPosition( b27 );
            _board[1, 7] = new BoardPosition( b28 );

            _board[2, 0] = new BoardPosition( b31 );
            _board[2, 1] = new BoardPosition( b32 );
            _board[2, 2] = new BoardPosition( b33 );
            _board[2, 3] = new BoardPosition( b34 );
            _board[2, 4] = new BoardPosition( b35 );
            _board[2, 5] = new BoardPosition( b36 );
            _board[2, 6] = new BoardPosition( b37 );
            _board[2, 7] = new BoardPosition( b38 );

            _board[3, 0] = new BoardPosition( b41 );
            _board[3, 1] = new BoardPosition( b42 );
            _board[3, 2] = new BoardPosition( b43 );
            _board[3, 3] = new BoardPosition( b44 );
            _board[3, 4] = new BoardPosition( b45 );
            _board[3, 5] = new BoardPosition( b46 );
            _board[3, 6] = new BoardPosition( b47 );
            _board[3, 7] = new BoardPosition( b48 );

            _board[4, 0] = new BoardPosition( b51 );
            _board[4, 1] = new BoardPosition( b52 );
            _board[4, 2] = new BoardPosition( b53 );
            _board[4, 3] = new BoardPosition( b54 );
            _board[4, 4] = new BoardPosition( b55 );
            _board[4, 5] = new BoardPosition( b56 );
            _board[4, 6] = new BoardPosition( b57 );
            _board[4, 7] = new BoardPosition( b58 );

            _board[5, 0] = new BoardPosition( b61 );
            _board[5, 1] = new BoardPosition( b62 );
            _board[5, 2] = new BoardPosition( b63 );
            _board[5, 3] = new BoardPosition( b64 );
            _board[5, 4] = new BoardPosition( b65 );
            _board[5, 5] = new BoardPosition( b66 );
            _board[5, 6] = new BoardPosition( b67 );
            _board[5, 7] = new BoardPosition( b68 );

            _board[6, 0] = new BoardPosition( b71 );
            _board[6, 1] = new BoardPosition( b72 );
            _board[6, 2] = new BoardPosition( b73 );
            _board[6, 3] = new BoardPosition( b74 );
            _board[6, 4] = new BoardPosition( b75 );
            _board[6, 5] = new BoardPosition( b76 );
            _board[6, 6] = new BoardPosition( b77 );
            _board[6, 7] = new BoardPosition( b78 );

            _board[7, 0] = new BoardPosition( b81 );
            _board[7, 1] = new BoardPosition( b82 );
            _board[7, 2] = new BoardPosition( b83 );
            _board[7, 3] = new BoardPosition( b84 );
            _board[7, 4] = new BoardPosition( b85 );
            _board[7, 5] = new BoardPosition( b86 );
            _board[7, 6] = new BoardPosition( b87 );
            _board[7, 7] = new BoardPosition( b88 );
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

        private Bitmap _bitmap;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.Key == Key.D1 )
            {
                Point cursor;
                GetCursorPos( out cursor );
                _boardTopLeft = cursor;
                _boardBottomRight = new Point(cursor.X + (40*8), cursor.Y + (40*8));

                this.RefreshBoardFromBitmap();
            }

            if ( e.Key == Key.D2 )
            {
                this.RefreshBoardFromBitmap();
            }

            if ( e.Key == Key.D3 )
            {
                FindAndApplySolution();
            }

            if ( e.Key == Key.D4 )
            {
                this.RefreshBoardFromBitmap();

                int tryCounter = 0;
                while (_unknownCount > 0)
                {
                    this.RefreshBoardFromBitmap();
                    Thread.Sleep( 100 );

                    if ((tryCounter++) > 10)
                        break;
                }

                Thread.Sleep( 100 );
                FindAndApplySolution();
            }
            
            if ( e.Key == Key.D5 )
            {
                RefreshBoardFromBitmap();

                int tryCounter = 0;
                while (_unknownCount > 0)
                {
                    this.RefreshBoardFromBitmap();
                    Thread.Sleep( 100 );

                    if ((tryCounter++) > 10)
                        break;
                }

                Thread.Sleep( 100 );
                FindAndApplySolution(chooseRandom: true);
            }

            if ( e.Key == Key.H )
            {
                UseHypercube();
            }
          

            base.OnKeyDown(e);
        }

        private void UpdateBitmap()
        {
            var size = new System.Drawing.Size(_boardBottomRight.X - _boardTopLeft.X, _boardBottomRight.Y - _boardTopLeft.Y);

            _bitmap = new Bitmap(size.Width, size.Height);
            Graphics gra = Graphics.FromImage(_bitmap);
            gra.CopyFromScreen(_boardTopLeft, new Point(0, 0), size);            

            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                _bitmap.GetHbitmap(), 
                IntPtr.Zero,
                Int32Rect.Empty, 
                BitmapSizeOptions.FromEmptyOptions());

            image1.Source = bs;            
        }

        private static Random random = new Random();
        private void FindAndApplySolution(bool chooseRandom = false)
        {
            var sf = new NaiveBestSolutionFinder( _board );
            var sol = sf.FindSolutions();

            Point currentPos;
            GetCursorPos( out currentPos );

            if ( sol != null )
            {
                if (chooseRandom)
                {
                    int randomSolution = random.Next(0, sol.Length - 1);
                    this.ApplySolution(sol[randomSolution]);
                }
                else
                {
                    ApplySolution(sol.First());
                }
            }
            else
            {
                solution.Content = "No solution found";
            }
        }

        private void ApplySolution( Solution sol )
        {
            solution.Content = string.Format( "Solution: (weight:{4}) x:{0}, y:{1} to x{2}, y:{3}",
                                              sol.ArrayPosition1.X,
                                              sol.ArrayPosition1.Y,
                                              sol.ArrayPosition2.X,
                                              sol.ArrayPosition2.Y,
                                              sol.Weight );

            SetCursorPos( _boardTopLeft.X + 20 + ( sol.ArrayPosition1.X * 40 ),
                          _boardTopLeft.Y + 20 + ( sol.ArrayPosition1.Y * 40 ) );

            mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
            Thread.Sleep( 50 );
            mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );

            Thread.Sleep( 100 );

            SetCursorPos( _boardTopLeft.X + 20 + ( sol.ArrayPosition2.X * 40 ),
                          _boardTopLeft.Y + 20 + ( sol.ArrayPosition2.Y * 40 ) );

            mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
            Thread.Sleep( 50 );
            mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );

            SetForegroundWindow(new WindowInteropHelper(this).Handle);       
        }

        private int _unknownCount;
        private int _refreshCount;

        private BitmapImage BitmapToImageSource(Bitmap bitmap, ImageFormat imgFormat)
        {
            if (bitmap == null)
                return null;

            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, imgFormat);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void RefreshBoardFromBitmap()
        {
            this.UpdateBitmap();

            _unknownCount = 0;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int setX = 40*x;
                    int setY = 40*y;

                    int screenX = _boardTopLeft.X + setX;
                    int screenY = _boardTopLeft.Y + setY;
                    //SetCursorPos(screenX, screenY);

                    //Thread.Sleep(500);

                    var averageColor = ColorUtility.GetAveragePieceColor(_bitmap, setX, setY);
                    
                    var matches = BoardPiece.FindMatches(averageColor);
                    var closestMatch = matches.GetClosestMatch().BoardPiece;

                    _board[y, x].Facade.Fill = new SolidColorBrush(ConvertToMediaColor(averageColor));
                    _board[y, x].Facade.ToolTip = closestMatch.Name;

                    if (closestMatch.GetImage() != null)
                        _board[y, x].Facade.Fill = new ImageBrush(this.BitmapToImageSource(closestMatch.GetImage(), ImageFormat.Bmp));
                    
                    
                    if (closestMatch == BoardPiece.Unknown)
                    {
                        _unknownCount++;
                        _board[y, x].Facade.Stroke = Brushes.Red;
                        _board[y, x].Facade.StrokeThickness = 3;
                    }
                    else
                    {
                        _board[y, x].Facade.Stroke = Brushes.Black;
                        _board[y, x].Facade.StrokeThickness = 1;
                    }

                    _board[y, x].SetPiece(closestMatch, new Point(setX, setY), y, x);
                }
            }
        }

        private void UseHypercube()
        {
            var ev = FindHypercube();
            if ( ev.HyperCubePosition == null )
            {
                return;
            }

            var top = _board.TopOf( ev.HyperCubePosition );
            var right = _board.RightOf( ev.HyperCubePosition );
            var bottom = _board.BottomOf( ev.HyperCubePosition );
            var left = _board.LeftOf( ev.HyperCubePosition );

            Tuple<Surround,int>[] weights = new[]
            {
                new Tuple<Surround, int>( Surround.Top, ( top == null ) ? 0 : ev.RootBoardPieceWeights[top.Piece.RootBoardPiece] ),
                new Tuple<Surround, int>( Surround.Right, ( right == null ) ? 0 : ev.RootBoardPieceWeights[right.Piece.RootBoardPiece] ),
                new Tuple<Surround, int>( Surround.Bottom, ( bottom == null ) ? 0 : ev.RootBoardPieceWeights[bottom.Piece.RootBoardPiece] ),
                new Tuple<Surround, int>( Surround.Left, ( left == null ) ? 0 : ev.RootBoardPieceWeights[left.Piece.RootBoardPiece] ),
            };

            var moveHyperCube = weights.OrderByDescending( m => m.Second ).Select( m => m.First ).First();

            Point moveTo = new Point();
            switch ( moveHyperCube )
            {
                case Surround.Top:
                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X, ev.HyperCubePosition.ArrayPosition.Y - 1 );
                    break;

                case Surround.Right:
                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X + 1, ev.HyperCubePosition.ArrayPosition.Y );
                    break;

                case Surround.Bottom:
                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X, ev.HyperCubePosition.ArrayPosition.Y + 1 );
                    break;

                case Surround.Left:
                    moveTo = new Point( ev.HyperCubePosition.ArrayPosition.X - 1, ev.HyperCubePosition.ArrayPosition.Y );
                    break;
            }

            var solution = new Solution( ev.HyperCubePosition.ArrayPosition, moveTo );
            ApplySolution( solution );
        }

        enum Surround
        {
            Top,
            Right,
            Bottom,
            Left
        }


        private HyperCubeEvaluation FindHypercube()
        {
            HyperCubeEvaluation output = new HyperCubeEvaluation();

            for ( int y = 0; y < 8; y++ )
            {
                for ( int x = 0; x < 8; x++ )
                {
                    int setX = _boardCalibration.X + ( 40 * x );
                    int setY = _boardCalibration.Y + ( 40 * y );

                    var dc = CreateDC( "Display", null, null, IntPtr.Zero );

                    var color = GetColorPointsFrom( dc, setX, setY );

                    DeleteDC( dc );

                    var boardPiece = BoardPiece.FindMatch( color );
                    
                    if ( boardPiece == BoardPiece.HyperCube )
                    {
                        output.HyperCubePosition = new BoardPosition();
                        output.HyperCubePosition.SetPiece( boardPiece, new Point( setX, setY ), y, x );
                    }

                    if ( ! output.RootBoardPieceWeights.ContainsKey( boardPiece.RootBoardPiece ) )
                    {
                        output.RootBoardPieceWeights[boardPiece.RootBoardPiece] = boardPiece.Weight;
                    }
                    else
                    {
                        output.RootBoardPieceWeights[boardPiece.RootBoardPiece] += boardPiece.Weight;
                    }
                }
            }

            return output;
        }

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

        private ColorPoints GetColorPointsFromBitmap(int x, int y)
        {
            System.Drawing.Color centerColor = _bitmap.GetPixel(x, y);
            var leftColor = _bitmap.GetPixel(x + ColorPoints.LeftMostOffset, y);
            var rightColor = _bitmap.GetPixel(x + ColorPoints.RightMostOffset, y );

            var topMaxColor = _bitmap.GetPixel(x, y + ColorPoints.TopMostOffset );
            var bottomMaxColor = _bitmap.GetPixel(x, y + ColorPoints.BottomMostOffset );

            var topMidColor = _bitmap.GetPixel(x, y - ColorPoints.TopMiddleOffset );
            var bottomMidColor = _bitmap.GetPixel(x, y + ColorPoints.BottomMiddleOffset );

            var colorPoints = new ColorPoints
            {
                Center = ConvertToDrawingColor(ConvertToMediaColor(centerColor)),
                TopMost = ConvertToDrawingColor(ConvertToMediaColor(topMaxColor)),
                TopMiddle = ConvertToDrawingColor(ConvertToMediaColor(topMidColor)),
                BottomMost = ConvertToDrawingColor(ConvertToMediaColor(bottomMaxColor)),
                BottomMiddle = ConvertToDrawingColor(ConvertToMediaColor(bottomMidColor)),
                LeftMost = ConvertToDrawingColor(ConvertToMediaColor(leftColor)),
                RightMost = ConvertToDrawingColor(ConvertToMediaColor(rightColor))
            };

            return colorPoints;
        }

        private ColorPoints GetColorPointsFrom( IntPtr dc, int x, int y )
        {          
            var centerColor = GetPixelColorAt( dc, x, y );
            var leftColor = GetPixelColorAt( dc, x + ColorPoints.LeftMostOffset, y );
            var rightColor = GetPixelColorAt( dc, x + ColorPoints.RightMostOffset, y );

            var topMaxColor = GetPixelColorAt( dc, x, y + ColorPoints.TopMostOffset );
            var bottomMaxColor = GetPixelColorAt( dc, x, y + ColorPoints.BottomMostOffset );

            var topMidColor = GetPixelColorAt( dc, x, y - ColorPoints.TopMiddleOffset );
            var bottomMidColor = GetPixelColorAt( dc, x, y + ColorPoints.BottomMiddleOffset );

            var colorPoints = new ColorPoints
            {
                Center = ConvertToDrawingColor( centerColor ),
                TopMost = ConvertToDrawingColor( topMaxColor ),
                TopMiddle = ConvertToDrawingColor( topMidColor ),

                BottomMost = ConvertToDrawingColor( bottomMaxColor ),
                BottomMiddle = ConvertToDrawingColor( bottomMidColor ),

                LeftMost = ConvertToDrawingColor( leftColor ),
                RightMost = ConvertToDrawingColor( rightColor )
            };

            return colorPoints;
        }

        private static System.Drawing.Color ConvertToDrawingColor( Color color )
        {
            return System.Drawing.Color.FromArgb( 255, Convert.ToByte( color.R ), Convert.ToByte( color.G ), Convert.ToByte( color.B ) );
        }

        public static Color ConvertToMediaColor( System.Drawing.Color color )
        {
            return Color.FromArgb( 255, Convert.ToByte( color.R ), Convert.ToByte( color.G ), Convert.ToByte( color.B ) );
        }

        private static Color GetDominantColor( Color[] colors )
        {
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            foreach ( Color color in colors )
            {
                r += color.R;
                g += color.G;
                b += color.B;
            }

            //Calculate average
            r /= colors.Length;
            g /= colors.Length;
            b /= colors.Length;

            var newColor = Color.FromArgb( 255, Convert.ToByte( r ), Convert.ToByte( g ), Convert.ToByte( b ) );
            return newColor;
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


            var bitmapTopLeftY = Canvas.GetTop(image1) + (rectangleY*40);
            var bitmapTopLeftX = Canvas.GetLeft(this.image1) + (rectangleX*40);

            Canvas.SetTop(imageHighlight, bitmapTopLeftY);
            Canvas.SetLeft(imageHighlight, bitmapTopLeftX);

            var averageColor = ColorUtility.GetAveragePieceColor(_bitmap, rectangleX*40, rectangleY*40);
            var matches = BoardPiece.FindMatches(averageColor);

            var closestMatch = matches.GetClosestMatch();

            listBox1.Items.Clear();
            foreach (var match in matches)
            {
                listBox1.Items.Add(new ListBoxMatch(match, closestMatch.BoardPiece.Name == match.BoardPiece.Name));
            }
        }

        private void ListBox1_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var mp = (System.Windows.Controls.ListBox) sender;
            var item = (ListBoxMatch) mp.SelectedItem;
            if (item == null)
                return;

            rectSourcePiece.Fill = new ImageBrush(this.BitmapToImageSource(item.MatchPair.BoardPiece.GetImage(), ImageFormat.Bmp));

            var piecePoint = new Point(this.highlightedPiece.X*40, this.highlightedPiece.Y*40);
            var boardPiece = this._bitmap.Clone(
                new Rectangle(piecePoint, new System.Drawing.Size(40, 40)),
                PixelFormat.DontCare);

            rectBoardPiece.Fill = new ImageBrush(this.BitmapToImageSource(boardPiece, ImageFormat.Bmp));

            lblColorDist.Content = item.MatchPair.Weight.First.ToString("##.####");
            lblLuminanceDist.Content = item.MatchPair.Weight.Second.ToString("##.####");

            rectBoardPieceAverageColor.Fill = new SolidColorBrush(ConvertToMediaColor(ColorUtility.GetAveragePieceColor(boardPiece, 0, 0)));
            rectSourcePieceAverageColor.Fill = new SolidColorBrush(ConvertToMediaColor(ColorUtility.GetAveragePieceColor(item.MatchPair.BoardPiece.GetImage(), 0, 0)));

            rectBoardPieceAverageLuminance.Fill = new SolidColorBrush(ConvertToMediaColor(ColorUtility.GetAveragePieceHSL(boardPiece, 0, 0)));
            rectSourcePieceAverageLuminance.Fill = new SolidColorBrush(ConvertToMediaColor(ColorUtility.GetAveragePieceHSL(item.MatchPair.BoardPiece.GetImage(), 0, 0)));
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
