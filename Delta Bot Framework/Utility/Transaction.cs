using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleBotTemplate
{
    public class Transaction
    {
        private string txn_id;
        private DateTime txn_dt;
        private string chnl_id;
        private string txn_conv_id;
        private string txn_txt;
        private string txn_state_txt;
        private string txn_ctr;
        private string sel_txt;
        private string order_id;
        private string botname;

        public string TxnId
        {
            get
            {
                return txn_id;
            }
            set
            {
                this.txn_id = value;
            }
        }
        public DateTime TxnDate
        {
            get
            {
                return txn_dt;
            }
            set
            {
                this.txn_dt = value;
            }
        }
        public string ChannelId
        {
            get
            {
                return chnl_id;
            }
            set
            {
                this.chnl_id = value;
            }

        }
        public string TxnConversationId
        {
            get
            {
                return txn_conv_id;
            }
            set
            {
                this.txn_conv_id = value;
            }

        }
        public string TxnText
        {
            get
            {
                return txn_txt;
            }
            set
            {
                this.txn_txt = value;
            }

        }
        public string TxnStateText
        {
            get
            {
                return txn_state_txt;
            }
            set
            {
                this.txn_state_txt = value;
            }

        }
        public string TxnCounter
        {
            get
            {
                return txn_ctr;
            }
            set
            {
                this.txn_ctr = value;
            }

        }
        public string SelText
        {
            get
            {
                return sel_txt;
            }
            set
            {
                this.sel_txt = value;
            }

        }
        public string OrderId
        {
            get
            {
                return order_id;
            }
            set
            {
                this.order_id = value;
            }

        }
        public string Bot_Nm
        {
            get {
                return this.botname;
            }
            set
            {
                this.botname = value;
            }
        }
    }
}
