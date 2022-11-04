using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace CharaGaming.BullInAChinaShop.Utils
{
    public class TweenChain
    {
        private readonly Queue<Sequence> _sequenceQueue = new Queue<Sequence>();

        public TweenChain() {}

        public void AddToQueue(Tween tween)
        {
            var sequence = DOTween.Sequence();
            sequence.Pause();
            sequence.Append(tween);
            _sequenceQueue.Enqueue(sequence);
            if (_sequenceQueue.Count == 1)
            {
                _sequenceQueue.Peek().Play();
            }

            sequence.OnComplete(OnComplete);
        }

        private void OnComplete()
        {
            _sequenceQueue.Dequeue();

            if (_sequenceQueue.Count > 0)
            {
                _sequenceQueue.Peek().Play();
            }
        }

        public bool IsRunning()
        {
            return (_sequenceQueue.Any());
        }

        public void KillAll()
        {
            foreach (var sequence in _sequenceQueue)
            {
                sequence.Kill();
            }

            _sequenceQueue.Clear();
        }
    }
}