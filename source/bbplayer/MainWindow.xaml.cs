﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Brushes=System.Windows.Media.Brushes;
using Color=System.Windows.Media.Color;
using KeyEventArgs=System.Windows.Input.KeyEventArgs;
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ( e.Key == Key.D1 )
            {
                Point cursor;
                GetCursorPos( out cursor );
                _mainWindow = cursor;
            }

            if ( e.Key == Key.D2 )
            {
                Point cursor;
                GetCursorPos(out cursor);
                _boardCalibration = cursor;
                label1.Content = string.Format("Calibration: X:{0}; Y:{1}", cursor.X, cursor.Y);
            }

            if ( e.Key == Key.D3 )
            {
                //JetBrains.dotTrace.Api.CPUProfiler.Start();

                RefreshBoard();

                //JetBrains.dotTrace.Api.CPUProfiler.StopAndSaveSnapShot();
            }

            if ( e.Key == Key.D4 )
            {
                FindAndApplySolution();
            }

            if ( e.Key == Key.D5 )
            {
                RefreshBoard();

                if ( _unknownCount > 0 )
                {
                    RefreshBoard();
                    Thread.Sleep( 100 );
                }

                Thread.Sleep( 100 );
                FindAndApplySolution();
            }
          

            base.OnKeyDown(e);
        }

        private void FindAndApplySolution()
        {
            var sf = new NaiveBestSolutionFinder( _board );
            var sol = sf.FindSolution();

            Point currentPos;
            GetCursorPos( out currentPos );

            if ( sol != null )
            {
                solution.Content = string.Format( "Solution: (weight:{4}) x:{0}, y:{1} to x{2}, y:{3}",
                                                  sol.ArrayPosition1.X,
                                                  sol.ArrayPosition1.Y,
                                                  sol.ArrayPosition2.X,
                                                  sol.ArrayPosition2.Y,
                                                  sol.Weight );

                SetCursorPos( _boardCalibration.X + ( sol.ArrayPosition1.X * 40 ),
                              _boardCalibration.Y + ( sol.ArrayPosition1.Y * 40 ) );

                mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
                Thread.Sleep( 50 );
                mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );

                Thread.Sleep( 150 );

                SetCursorPos( _boardCalibration.X + ( sol.ArrayPosition2.X * 40 ),
                              _boardCalibration.Y + ( sol.ArrayPosition2.Y * 40 ) );

                mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
                Thread.Sleep( 50 );
                mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );


                SetCursorPos( _mainWindow.X, _mainWindow.Y );
                mouse_event( (uint)MouseEventFlags.LEFTDOWN, 0, 0, 0, UIntPtr.Zero );
                Thread.Sleep( 50 );
                mouse_event( (uint)MouseEventFlags.LEFTUP, 0, 0, 0, UIntPtr.Zero );
                    
            }
            else
            {
                solution.Content = "No solution found";
            }
        }

        private int _unknownCount;
        private int _refreshCount;

        private void RefreshBoard()
        {
            _unknownCount = 0;

            for ( int y = 0; y < 8; y++ )
            {
                for ( int x = 0; x < 8; x++ )
                {
                    int setX = _boardCalibration.X + (40*x);
                    int setY = _boardCalibration.Y + (40*y);

                    //SetCursorPos( setX, setY );

                    var color = GetPixelColorAt( setX, setY );
                        
                    var boardPiece = BoardPiece.FindMatch( color );

                    _board[y, x].Facade.Fill = new SolidColorBrush( color );
                    _board[y, x].Facade.ToolTip = boardPiece.Name;
                    if ( boardPiece == BoardPiece.Unknown )
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
                    
                    _board[y, x].SetPiece( boardPiece, new Point( setX, setY ), y, x );
                }
            }
        }

        private Color GetPixelColorAt( int x, int y )
        {
            var dc = GetDC( IntPtr.Zero );
            var pixel = GetPixel( dc, x, y );
            var color = ColorTranslator.FromWin32( pixel );

            return Color.FromArgb( color.A, color.R, color.G, color.B );
        }

        private void CapturePixelColorUnderCursor( object sender, EventArgs e )
        {
            Point cursorPos;
            GetCursorPos( out cursorPos );

            var dc = GetDC( IntPtr.Zero );
            var p = GetPixel( dc, Convert.ToInt32( cursorPos.X ), Convert.ToInt32( cursorPos.Y ) );
            var c = ColorTranslator.FromWin32( p );

            bgColorStatus.Text =
                string.Format( "H:{0}; S:{1}; B:{2} | ", c.GetHue(), c.GetSaturation(), c.GetBrightness() )
                + c.ToString();
            cursorXStatus.Text = "X: " + cursorPos.X;
            cursorYStatus.Text = "Y: " + cursorPos.Y;

            var bgColor = System.Windows.Media.Color.FromArgb( c.A, c.R, c.G, c.B );
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

        [DllImport( "User32.dll" )]
        private static extern IntPtr GetDC( IntPtr wnd );

        [DllImport( "User32.dll" )]
        private static extern void ReleaseDC( IntPtr dc );

        [DllImport( "User32.dll" )]
        private static extern IntPtr SetCapture( IntPtr hWnd );

        [DllImport( "User32.dll" )]
        private static extern int ReleaseCapture( IntPtr hWnd );

        [DllImport("user32.dll")]
        static extern void mouse_event( uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo );

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

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        private static extern bool RegisterHotKey( IntPtr hWnd, int id, uint fsModifiers, uint vk );

    }

}
