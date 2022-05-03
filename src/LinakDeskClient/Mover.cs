using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LinakDeskClient
{
    public class Mover
    {
        public Mover(LinakDesk desk)
        {
            Desk = desk ?? throw new ArgumentNullException(nameof(desk));
            Desk.OnHeightOrSpeedChanged += Desk_OnHeightOrSpeedChanged;
            Desk.OnReceivedError += Desk_OnReceivedError;
        }

        int errorsCount = 0;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        readonly AutoResetEvent newHeightEvent = new AutoResetEvent(false);

        private void Desk_OnReceivedError()
        {
            Interlocked.Increment(ref errorsCount);
        }

        /// <summary>
        /// Gets or sets offset to stop before reaching final height to time to stop (lag and inertia). If set to 0, then stopping will begin after reaching final height.
        /// </summary>
        /// <remarks>
        /// This default value works pretty fine with my desk. Maybe working out some algorithm based on speed would be more accurate.
        /// </remarks>
        public DeskHeight InertiaStopOffset { get; set; } = DeskHeight.FromCm(0.7);

        private void Desk_OnHeightOrSpeedChanged(DeskHeightAndSpeed value)
        {
            newHeightEvent.Set();
        }

        public LinakDesk Desk { get; }

        public async Task MoveToMemoryPositionAsync(int memoryPositionNumber)
        {
            var memoryPosition = Desk.State.Memory.GetByNumber(memoryPositionNumber);
            if (!memoryPosition.IsSet) return;
            await MoveToPositionAsync(memoryPosition.Height);
        }

        public async Task MoveToPositionAsync(DeskHeight height)
        {
            await semaphore.WaitAsync();
            try
            {
                bool goUp = height.Value > Desk.State.Height.Value;

                Stopwatch stopwatch = null;
                TimeSpan sendMoveCommandEvery = TimeSpan.FromMilliseconds(500);
                int lastErrorsCount = errorsCount;

                while(true)
                {
                    // reached final position?
                    if ((goUp && height.Value - InertiaStopOffset.Value <= Desk.State.Height.Value) || (!goUp && height.Value + InertiaStopOffset.Value >= Desk.State.Height.Value))
                    {
                        await Desk.MoveStopAsync();
                        return;
                    }

                    // sending move command once every 500ms is enough to keep desk movement without stopping
                    if (stopwatch == null || stopwatch.Elapsed > sendMoveCommandEvery)
                    {
                        // move
                        if (goUp)
                        {
                            await Desk.MoveUpAsync();
                        }
                        else
                        {
                            await Desk.MoveDownAsync();
                        }
                        (stopwatch ?? (stopwatch = Stopwatch.StartNew())).Restart();
                    }

                    // check for errors (collision etc.)
                    if(errorsCount > lastErrorsCount)
                    {
                        Console.WriteLine("Debug: Stopping because an error was received during operation.");
                        return;
                    }

                    // wait for updated position; this should happen quite often as controller sends 5+ updates per second when table is moving
                    newHeightEvent.WaitOne(TimeSpan.FromMilliseconds(100));
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
