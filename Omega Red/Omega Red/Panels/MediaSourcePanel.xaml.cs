using Omega_Red.Models;
using Omega_Red.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Omega_Red.Panels
{
    /// <summary>
    /// Interaction logic for MediaSourcePanel.xaml
    /// </summary>
    public partial class MediaSourcePanel : UserControl
    {
        public string SymbolicLink { get; private set; }

        private MediaSourceInfo m_MediaSourceInfo;

        public MediaSourcePanel()
        {
            InitializeComponent();
        }

        public void releaseResource()
        {
            var lVideoPanel = m_ContentPresenter.Content as VideoPanel;
            
            m_ContentPresenter.Content = null;

            if(lVideoPanel != null)
            {
                lVideoPanel.Dispose();

                lVideoPanel = null;
            }
        }

        internal MediaSourcePanel(VideoPanel aVideoPanel, MediaSourceInfo a_MediaSourceInfo)
        {
            InitializeComponent();

            m_ContentPresenter.Content = aVideoPanel;

            Width = aVideoPanel.VideoWidth;

            Height = aVideoPanel.VideoHeight;

            SymbolicLink = aVideoPanel.SymbolicLink;

            m_MediaSourceInfo = a_MediaSourceInfo;


            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (ThreadStart)delegate ()
            {
                updatePosition();
            });
        }

        void onDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            Canvas lParentCanvas = Parent as Canvas;

            if (lParentCanvas == null)
                return;

            double lLeftPos = Canvas.GetLeft(this);

            double lTopPos = Canvas.GetTop(this);

            //Move the Thumb to the mouse position during the drag operation
            double yadjust = this.Height + e.VerticalChange;
            double xadjust = this.Width + e.HorizontalChange;
            if ((xadjust >= 0) && (yadjust >= 0) &&
                ((lLeftPos + xadjust) <= lParentCanvas.ActualWidth) && ((lTopPos + yadjust) <= lParentCanvas.ActualHeight))
            {
                this.Width = xadjust;
                this.Height = yadjust;

                updatePosition();
            }
        }
        private void updatePosition()
        {
            Canvas lParentCanvas = Parent as Canvas;

            if (lParentCanvas == null)
                return;

            double lLeftPos = Canvas.GetLeft(this);

            double lLeftProp = lLeftPos / lParentCanvas.ActualWidth;

            double lRightProp = (lLeftPos + Width) / lParentCanvas.ActualWidth;

            double lTopPos = Canvas.GetTop(this);

            double lTopProp = lTopPos / lParentCanvas.ActualHeight;

            double lBottomProp = (lTopPos + Height) / lParentCanvas.ActualHeight;


            Managers.MediaRecorderManager.Instance.setPosition(
                SymbolicLink,
                (float)lLeftProp,
                (float)lRightProp,
                (float)lTopProp,
                (float)lBottomProp);
        }

        void onDragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            myThumb.Background = Brushes.Orange;
        }

        void onDragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            myThumb.Background = Brushes.Blue;
        }






        bool lPress = false;

        Point mPrevPoint = new Point();

        private void UserControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lPress = true;

            Canvas lParentCanvas = Parent as Canvas;

            if (lParentCanvas == null)
                return;

            mPrevPoint = Mouse.GetPosition(lParentCanvas);

            var lcurrZIndex = Canvas.GetZIndex(this);

            List<int> l = new List<int>();

            foreach (var item in lParentCanvas.Children)
            {
                var lZIndex = Canvas.GetZIndex((UIElement)item);

                if ((lParentCanvas.Children.Count - 1) == lZIndex)
                {
                    Canvas.SetZIndex(this, lZIndex);

                    Canvas.SetZIndex((UIElement)item, lcurrZIndex);

                    break;
                }

            }

            //if (mMaxVideoRenderStreamCount > 0)
            //    mIEVRStreamControl.setZOrder(
            //        mIMFTopologyNode,
            //        mMaxVideoRenderStreamCount - 1);
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            lPress = false;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (lPress)
            {
                Canvas lParentCanvas = Parent as Canvas;

                if (lParentCanvas == null)
                    return;

                Point lPoint = Mouse.GetPosition(lParentCanvas);

                var ldiff = mPrevPoint - lPoint;

                double lLeftPos = Canvas.GetLeft(this);

                double lTopPos = Canvas.GetTop(this);

                double lnewLeftPos = lLeftPos - ldiff.X;

                if (lnewLeftPos >= 0 && lnewLeftPos <= (lParentCanvas.ActualWidth - Width))
                    Canvas.SetLeft(this, lnewLeftPos);


                double lnewTopPos = lTopPos - ldiff.Y;

                if (lnewTopPos >= 0 && lnewTopPos <= (lParentCanvas.ActualHeight - Height))
                    Canvas.SetTop(this, lnewTopPos);

                mPrevPoint = lPoint;

                updatePosition();
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            lPress = false;
        }
    }
}
