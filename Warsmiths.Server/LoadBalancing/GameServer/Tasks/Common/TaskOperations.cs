using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExitGames.Logging;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Tasks;
using Warsmiths.Common.Domain.VictoryPrizes;
using Task = Warsmiths.Common.Domain.Tasks.Task;

namespace Warsmiths.Server.GameServer.Tasks.Common
{
    public static class TaskOperations
    {
        public static void CloseThread(List<Task> taskList, Task exeption)
        {
            foreach (var task in taskList)
            {
                if (task != exeption && task.Status == TaskStatusTypesE.InProgress)
                {
                    task.Status = TaskStatusTypesE.Denied;
                }
            }
        }

        public static void OpenThread(List<Task> taskList)
        {
            foreach (var task in taskList)
            {
                task.Status = TaskStatusTypesE.InProgress;
            }
        }

        public static Task CheckTask(Player player, Character character, TaskTypesE taskType = TaskTypesE.Craft)
        {
            ILogger _log = LogManager.GetCurrentClassLogger();
            var tasklist = character.TasksList.Cast<Task>().ToList();
            var firstTask = tasklist.Find(x => x.Status == TaskStatusTypesE.InProgress);
            var currentTasks = tasklist.Where(x => x.Lvl == firstTask.Lvl).ToList();
         
            if (currentTasks.Find(x => x.Type == taskType) != null)
            {
                if (currentTasks.Count == 0) return null;
                foreach (var lastTask in tasklist)
                {
                    if (lastTask.Status == TaskStatusTypesE.Finished) continue;

                    if (lastTask.Type == TaskTypesE.ConquereLand)
                    {
                        lastTask.Status = TaskStatusTypesE.Finished;
                    }
                    else if (lastTask.Type == TaskTypesE.Harvest)
                    {
                        lastTask.Status = TaskStatusTypesE.Finished;
                    }
                    else if (lastTask.Type == TaskTypesE.CreateCharacter)
                    {
                        lastTask.Status = TaskStatusTypesE.Finished;
                    }
                    else
                    {
                        var task = lastTask as TaskCraftQuestReciept;
                        if (task != null)
                        {
                            var existTask = character.CompletedQuests.Find(x => x == task.RecieptName);
                            if (existTask != null)
                            {
                                lastTask.Status = TaskStatusTypesE.Finished;
                            }
                        }
                    }

                    if (lastTask.Status == TaskStatusTypesE.Finished && lastTask.Prize != null)
                    {
                        switch (lastTask.Prize.Type)
                        {
                            case VictoryPrizeTypeE.Equipment:
                                player.TryAddToInventory(lastTask.Prize.Item);
                                break;
                            case VictoryPrizeTypeE.Crystals:
                                player.Crystals += lastTask.Prize.KeysAmout;
                                break;
                            case VictoryPrizeTypeE.Keys:
                                player.Keys += lastTask.Prize.CrystalAmout;
                                break;
                        }
                    }
                    if (lastTask.Status == TaskStatusTypesE.Finished)
                    {
                        CloseThread(tasklist, lastTask);
                        OpenThread(tasklist.Where(x => x.Lvl == firstTask.Lvl + 1).ToList());
                        /// tas choose
                        
                        if (lastTask.Lvl > 1)
                        {
                            var tempt = tasklist.Where(x => x.Lvl == lastTask.Lvl - 1).ToList();
                            var tempComp = tempt.Find(x => x.Status == TaskStatusTypesE.Finished);
                            _log.Debug(lastTask.Name);
                            foreach (var a in tempt)
                            {
                                _log.Debug(a.Name + ":" + a.Status);
                            }
                            tempComp.FirstPathChoose = currentTasks[0].Name == lastTask.Name;
                            var tsaks = tasklist.Find(x => x.Name == tempComp.Name);
                            tsaks =  tempComp;
                        }


                     
                        return lastTask;
                    }
                }
            }
            return null;
        }

    }
}
