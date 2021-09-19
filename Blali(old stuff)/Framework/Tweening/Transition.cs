public class Transition<T> : Tweening.ITransition
{
    public Transition(Tweening.ITransitionType TransitionMethod, T StartValue, T EndValue, FinishedDelegate FinishAction)
    {
        // Check whether the tweening manager supports this type
        var type = typeof(T);
        if (Emmond.Program.Automator.m_mapManagedTypes.ContainsKey(type))
        {
            ManagedType = Emmond.Program.Automator.m_mapManagedTypes[type];
            this.StartValue = StartValue;
            this.EndValue = EndValue;
            Method = TransitionMethod;
            this.FinishAction = FinishAction;
            TransitionStater = Tweening.TransitionState.Idle;
            Value = StartValue;
        }
        else
        {
            throw new NotImplementedException("The tweening manager doesn't support " + typeof(T).Name + ".");
        }
    }

    public Transition()
    {
        // Check whether the tweening manager supports this type
        var type = typeof(T);
        if (Emmond.Program.Automator.m_mapManagedTypes.ContainsKey(type))
        {
            ManagedType = Emmond.Program.Automator.m_mapManagedTypes[type];
            TransitionStater = Tweening.TransitionState.Idle;
        }
        else
        {
            throw new NotImplementedException("The tweening manager doesn't support " + typeof(T).Name + ".");
        }
    }

    public void Update(GameTime gameTime)
    {
        if (Enabled & TransitionStater == Tweening.TransitionState.InProgress)
        {
            Timer += gameTime.ElapsedGameTime.TotalMilliseconds;

            // Calculate values
            double percentage;
            bool completed;
            Method.onTimer(Timer, out percentage, out completed);

            // Set value
            Value = (T)ManagedType.copy(ManagedType.getIntermediateValue(StartValue, EndValue, percentage));
            if (completed)
            {
                var switchExpr = Repeat;
                switch (switchExpr)
                {
                    case global::Emmond.Framework.Tweening.RepeatJob.None:
                        {
                            StopTransition();
                            break;
                        }

                    case global::Emmond.Framework.Tweening.RepeatJob.Reverse:
                        {
                            // Swap Start and Stop values
                            T tEnd = (T)ManagedType.copy(EndValue);
                            EndValue = (T)ManagedType.copy(StartValue);
                            StartValue = tEnd;
                            // Reset Values
                            Value = (T)ManagedType.copy(StartValue);
                            Timer = 0;
                            break;
                        }

                    case global::Emmond.Framework.Tweening.RepeatJob.JumpBack:
                        {
                            // Reset Values
                            Value = (T)ManagedType.copy(StartValue);
                            Timer = 0;
                            break;
                        }
                }
            }
        }
    }

    private void StopTransition()
    {
        TransitionStater = Tweening.TransitionState.Done;
        if (FinishAction is object)
            FinishAction.Invoke(this);
        TransitionCompletedEvent?.Invoke(this, new EventArgs());
    }

    public T StartValue { get; set; }
    public T EndValue { get; set; }
    public T Value { get; set; }
    public Tweening.ITransitionType Method { get; set; }
    public FinishedDelegate FinishAction { get; set; } // A delegate to be executed when the transition is complete/the transition loops
    public bool Enabled { get; set; } = true;
    public Tweening.RepeatJob Repeat { get; set; } = Tweening.RepeatJob.None;
    public Tweening.TransitionState TransitionStater { get; set; }

    private int Timer; // Keeps track of the elapsed time
    private Tweening.IManagedType ManagedType; // Interface for converting/calculating values for the specified type

    public event TransitionCompletedEventEventHandler TransitionCompletedEvent;

    public delegate void TransitionCompletedEventEventHandler(object sender, EventArgs e); // An event to be executed when the transition is complete/the transition loops

    public delegate void FinishedDelegate(Transition<T> sender); // A delegate to be executed when the transition is complete/the transition loops
}
