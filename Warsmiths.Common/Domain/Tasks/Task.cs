using System.Collections.Generic;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.VictoryPrizes;
using Warsmiths.Common.Utils;

namespace Warsmiths.Common.Domain.Tasks
{

    public class Task : IEntity
    {
        public TaskStatusTypesE Status;
        public TaskTypesE Type;
        public VictoryPrize Prize;
        public int CraftExp;
        public int Exp;
        public CommonClasses.IntPos Position;
        public List<CommonClasses.IntPos> FirstPath;
        public List<CommonClasses.IntPos> SecondPath;
        public bool FirstPathChoose;
        public int Lvl;
        public string Prefix;

        public Task()
        {
            FirstPath = new List<CommonClasses.IntPos>();
            SecondPath = new List<CommonClasses.IntPos>();
            FirstPathChoose = true;
        }
    }
}
