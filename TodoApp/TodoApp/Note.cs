using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Taskodo
{
    public class Note
    {
        DateTime _startTime;
        DateTime _endTime;
        String _message;
        NoteStatus _status = NoteStatus.Started;

        public enum NoteStatus { Started, Completed, Note };

        public Note(String msg)
        {
            _message = msg;
            _startTime = DateTime.Now;
        }

        public Note(String msg, NoteStatus status)
        {
            _message = msg;
            _startTime = DateTime.Now;
            _status = status;
        }

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        }

        public String Message
        {
            get
            {
                return _message;
            }
        }

        public NoteStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public void end()
        {
            if (_status != NoteStatus.Completed)
            {
                _endTime = DateTime.Now;
                _status = NoteStatus.Completed;
            }
        }
    }
}
