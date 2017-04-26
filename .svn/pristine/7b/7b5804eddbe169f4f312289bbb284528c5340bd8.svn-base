using System;
using Atlass.LAS.Lib.Types;
using Atlass.LAS.Lib.Types.Interface;

namespace Atlass.LAS.Lib.Operations
{
    public class TcLasSort<T> : TiOperation where T : TiLasGPS
    {
        /// <summary>
        /// Callback for any message to be passed to the parent thread.
        /// </summary>
        public override event EventHandler<TcMessageEventArgs> OnMessage;

        /// <summary>
        /// Callback for any error happened in the processing.
        /// </summary>
        public override event EventHandler<TcErrorEventArgs> OnError;

        /// <summary>
        /// Callback to notify the parent thread about process finish.
        /// </summary>
        public override event EventHandler<EventArgs> OnFinish;

        /// <summary>
        /// The percentage of progress to be used for notifying the caller.
        /// </summary>
        public override Double ProgressFrequency { protected get; set; }

        private T m_GPSPivot;
        private T m_GPSTemp;

        private Int64 m_IdxLeft;
        private Int64 m_IdxRight;

        public TcLasSort()
        {
            ProgressFrequency = 5;
        }
        //-----------------------------------------------------------------------------

        protected override void ReportMessage(String prmMessage)
        {
            if (OnMessage != null)
            {
                OnMessage(this, new TcMessageEventArgs(prmMessage));
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportFinished()
        {
            if (OnFinish != null)
            {
                OnFinish(this, new EventArgs());
            }
        }
        //-----------------------------------------------------------------------------

        protected override void ReportError(String prmError, Exception prmEx)
        {
            if (OnError != null)
            {
                OnError(this, new TcErrorEventArgs(prmError, prmEx));
            }
        }
        //-----------------------------------------------------------------------------

        /// <summary>
        /// Function to sort the LAS points according to the GPS time.
        /// </summary>
        /// <param name="prmLasPoints">LAS points to be sorted</param>
        /// <param name="prmLeft">Lower bound of the sub-group of points</param>
        /// <param name="prmRight">Upper bound of the sub-group of points</param>
        public void SortByGpsTime(T[] prmLasPoints, Int64 prmLeft, Int64 prmRight)
        {
            m_IdxLeft = prmLeft;
            m_IdxRight = prmRight;
            m_GPSPivot = prmLasPoints[(prmLeft + prmRight) / 2];

            while (m_IdxLeft <= m_IdxRight)
            {
                while (prmLasPoints[m_IdxLeft].GPSTime < m_GPSPivot.GPSTime)
                {
                    m_IdxLeft++;
                }

                while (prmLasPoints[m_IdxRight].GPSTime > m_GPSPivot.GPSTime)
                {
                    m_IdxRight--;
                }

                if (m_IdxLeft <= m_IdxRight)
                {
                    // Swap
                    m_GPSTemp = prmLasPoints[m_IdxLeft];
                    prmLasPoints[m_IdxLeft] = prmLasPoints[m_IdxRight];
                    prmLasPoints[m_IdxRight] = m_GPSTemp;

                    m_IdxLeft++;
                    m_IdxRight--;
                }
            }

            // Recursive calls
            if (prmLeft < m_IdxRight)
            {
                SortByGpsTime(prmLasPoints, prmLeft, m_IdxRight);
            }

            if (m_IdxLeft < prmRight)
            {
                SortByGpsTime(prmLasPoints, m_IdxLeft, prmRight);
            }
        }
        //-----------------------------------------------------------------------------

    }
    //-----------------------------------------------------------------------------

}
//-----------------------------------------------------------------------------