using ArchivalDataRepository.Context;
using ArchivalDataRepository.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using PMDataRepository.Context;
using PMDataRepository.CustomModels;
using PMDataRepository.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchivalFE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string GROUPID_CONFIGURATION_SETTING = "GroupID";

        private readonly ILogger _logger;
        private readonly PmdataArchivalContext _pmDataArchivalContext;
        private readonly PlayerManagementContext _playerManagementContext;
        private readonly IConfiguration _configuration;
        private CasinoGroup? casinoGroup; 
        private ObservableCollection<DataArchivalStatus> dataArchivalStatuses = new();
        private ObservableCollection<PlayersToBePurgedGroupBy> playersToBePurgedStatuses = new();
        private ObservableCollection<PlayersToBePurgedGroupBy> playersToBePurgedCompletedStatuses = new();
        private ObservableCollection<DataArchivalConfigDatagridBindingType> dataArchivalConfig = new();
        private ObservableCollection<PmDataArchivalJobReport> pmDataArchivalJobReports = new();
        public MainWindow(PmdataArchivalContext pmdataArchivalContext, PlayerManagementContext playerManagementContext, ILogger<MainWindow> logger, IConfiguration configuration)
        {
            InitializeComponent();
            _logger = logger;
            _pmDataArchivalContext = pmdataArchivalContext;
            _playerManagementContext = playerManagementContext;
            _configuration = configuration;
            GroupIDTextbox.Text = _configuration.GetValue<string>(GROUPID_CONFIGURATION_SETTING);
            PlayerGroupStackPanel.IsEnabled = false;
            DataArchivalStatusPMDataGrid.DataContext = dataArchivalStatuses;
            PlayersToBePurgedStatusPDADataGrid.DataContext = playersToBePurgedStatuses;
            PlayersToBePurgedCompletedPDADataGrid.DataContext = playersToBePurgedCompletedStatuses;
            DataArchivalConfigDatagrid.DataContext = dataArchivalConfig;
            PMDataArchivalJobReportDatagrid.DataContext = pmDataArchivalJobReports;
        }
        public async void CheckGroupInfoClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (int.TryParse(GroupIDTextbox.Text, out int groupIdInt))
                {
                    casinoGroup = await _playerManagementContext.CasinoGroups.FirstOrDefaultAsync(x => x.GroupId == groupIdInt);
                    if (casinoGroup == null)
                    {
                        GroupInfoTextbox.Text = "Group not found";
                        return;
                    }

                    GroupInfoTextbox.Text = $"SiteId:{casinoGroup.SiteId},GroupName:{casinoGroup.GroupName},Description:{casinoGroup.Description},Status:{casinoGroup.Status},PmLastUpdate:{casinoGroup.PmLastUpdate}";

                    PlayerGroupStackPanel.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error looking up group");
                GroupInfoTextbox.Text = "Error lookint up group";
            }
        }

        private async void AddPlayersToPlayerGroupClick(object sender, RoutedEventArgs e)
        {
            if (casinoGroup == null) return;
            AddPlayersToGroupProgressBar.Visibility = Visibility.Visible;
            AddPlayersToPlayerGroupButton.IsEnabled = false;
            try
            {
                if (int.TryParse(ExtractionIdTextbox.Text, out int extractionIdInt) && int.TryParse(UserIdTextbox.Text, out int userIdInt))
                {
                    var selectedDt = PlayerGroupDatePickerTextBox.SelectedDate;
                    //DateTime.TryParseExact(datetime
                    //    , Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern
                    //    , Thread.CurrentThread.CurrentCulture, System.Globalization.DateTimeStyles.None
                    //    , out DateTime selectedDt);

                    if(selectedDt >= DateTime.Now - TimeSpan.FromDays(365) || selectedDt == DateTime.MinValue)
                    {
                        _logger.LogWarning($"Selected Date is not far enough in the past: {selectedDt}. Aborting Player Insert");
                        return;
                    }
                    _logger.LogInformation($"Continuing with PlayerGroup insert with the following values: SelectedDate={selectedDt},GroupId={casinoGroup.GroupId},ExtractionId={extractionIdInt},UserId={userIdInt}");
                    await Task.Delay(10000);
                    
                    await _playerManagementContext.Database.ExecuteSqlAsync(@$"INSERT INTO [dbo].[PlayerGroup] (
	[PlayerId]
	,[GroupID]
	,[SiteID]
	,[ExtractionID]
	,[UserID]
	)
SELECT p.PlayerID
	,{casinoGroup.GroupId}
	,{casinoGroup.SiteId}
	,{extractionIdInt}
	,{userIdInt}
FROM Player p
WHERE NOT EXISTS (
		SELECT 1
		FROM playersession ps
		WHERE ps.PlayerID = p.PlayerID
			AND ps.EndTime > {selectedDt}
		)
	AND NOT EXISTS (
		SELECT 1
		FROM dbo.CouponRedeem cr
		WHERE cr.PlayerID = p.PlayerID
			AND cr.RedeemDate >{selectedDt}
		)
	AND NOT EXISTS (
		SELECT 1
		FROM dbo.CompAllocation ca
		JOIN dbo.Comp c ON c.CompID = ca.CompID
		WHERE ca.PlayerID = p.PlayerID
			AND c.AccountingDate > {selectedDt}
		)");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to add players to PlayerGroup");
            }
            finally
            {
                AddPlayersToGroupProgressBar.Visibility = Visibility.Hidden;
                AddPlayersToPlayerGroupButton.IsEnabled = true;
            }
        }
        private async void RefreshNumberOfPlayersInPlayerGroup(object sender, RoutedEventArgs e)
        {
            if (casinoGroup == null) return;
            RefreshNumberOfPlayersInPlayerGroupProgressBar.Visibility = Visibility.Visible;
            PlayerGroupCountTextbox.Text = "Checking...";
            try
            {
                var playercount = await _playerManagementContext.PlayerGroups.CountAsync(x => x.GroupId == casinoGroup.GroupId);
                PlayerGroupCountTextbox.Text = playercount.ToString();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unable to check player group count");
            }
            finally
            {
                RefreshNumberOfPlayersInPlayerGroupProgressBar.Visibility = Visibility.Hidden;

            }
        }
        private async void ExportPlayerGroupInfoToCSV(object sender, RoutedEventArgs e)
        {

            if (casinoGroup == null) return;
            ExportPlayerGroupInfoToCSVProgressBar.Visibility = Visibility.Visible;
            ExportPlayerGroupInfoToCSVButton.IsEnabled = false;
            try
            {
                CancellationTokenSource source = new CancellationTokenSource();
                var playerGroupInfoReportTask = _playerManagementContext.PlayerGroupInfoReports.FromSql($@"select pg.PlayerID, pn.FirstName, pn.LastName, ps.AccountingDate as PlayerSessionAccountingDate, cr.AccountingDate as CouponRedeemAccountingDate, c.AccountingDate as CompAccountingDate
from dbo.PlayerGroup pg
outer apply (select top 1 * from dbo.PlayerName pn where pn.PlayerID = pg.PlayerID order by pn.IsPreferredName desc) as pn
outer apply (select top 1 * from dbo.PlayerSession ps where ps.PlayerID = pg.PlayerID  order by ps.EndTime desc ) as ps
outer apply (select top 1 * from dbo.CouponRedeem cr where cr.PlayerID = pg.PlayerID order by cr.AccountingDate desc) as cr
outer apply (select top 1 c.* from dbo.CompAllocation ca join dbo.Comp c on c.CompID = ca.CompID where ca.PlayerID = pg.PlayerID order by c.AccountingDate desc ) as c
where pg.GroupID = {casinoGroup.GroupId}").ToListAsync(source.Token);
                //var playergroupexportTask =  _playerManagementContext.PlayerGroups.Where(x => x.GroupId == casinoGroup.GroupId).ToArrayAsync(source.Token);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "PlayerDataExport.csv";
                saveFileDialog.DefaultExt = ".csv";
               
                if (saveFileDialog.ShowDialog() == true)
                {
                    var path = saveFileDialog.FileName;
                    var pge = await playerGroupInfoReportTask;
                    using var streamwriter = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8, 1024); //100 megabyte buffer 
                    await streamwriter.WriteLineAsync("PlayerId, FirstName, LastName, PlayerSessionAccountingDate, CouponRedeemAccountingDate, CompAccountingDate");
                    foreach (var p in pge)
                    {
                        await streamwriter.WriteLineAsync($"{p.PlayerID},{p.FirstName},{p.LastName},{p.PlayerSessionAccountingDate},{p.CouponRedeemAccountingDate},{p.CompAccountingDate}");
                    }
                }
                else
                {
                    source.Cancel();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error exporting PlayerGroup info");
            }
            finally
            {
                ExportPlayerGroupInfoToCSVProgressBar.Visibility = Visibility.Hidden;
                ExportPlayerGroupInfoToCSVButton.IsEnabled = true ;
                
            }
        }
        private async void EnqueuePlayersForArchival(object sender, RoutedEventArgs e)
        {
            if (casinoGroup == null) return;
            try
            {
                if (int.TryParse(ExtractionIdTextbox.Text, out int extractionIdInt) && int.TryParse(UserIdTextbox.Text, out int userIdInt))
                {
                    ((Button)sender).IsEnabled = false;
                    EnqueuePlayersForArchivalProgressBar.Visibility = Visibility.Visible;
                    //run the stored proc
                    await _playerManagementContext.Database.ExecuteSqlAsync($"EXECUTE [dbo].[Proc_GroupDataArchive]   {casinoGroup.GroupId} ,{userIdInt},{extractionIdInt}");
                    
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error enqueing players for archival");
            }
            finally
            {
                ((Button)sender).IsEnabled = true;
                EnqueuePlayersForArchivalProgressBar.Visibility = Visibility.Hidden;
            }
        }
        private async void RefreshDashboard(object sender, RoutedEventArgs e)
        {
            try
            {
                ((Button)sender).IsEnabled = false;
                RefreshDashboardProgressBar.Visibility = Visibility.Visible;
                //get the status
                var x = await _playerManagementContext.DataArchivalStatuses.FromSql(@$"select s.StatusName, count(q.playerid) as Count
from dbo.DataArchivalStatusLookUp s
join dbo.DataArchivalQueue q on q.StatusID = s.ID
group by s.StatusName").ToListAsync();
                dataArchivalStatuses.Clear();
                foreach (var i in x)
                {
                    dataArchivalStatuses.Add(i);
                }
                MaxNumRecordsInTargetQueueTextBox.Text = await _pmDataArchivalContext.Database
                    .SqlQuery<string>($"select top 1 ConfigValue as Value from dbo.Config where ConfigName = 'MaxNumRecordsInTargetQueue'")
                    .FirstOrDefaultAsync();
                try
                {

                    var mqr = await _pmDataArchivalContext.Database
                         .SqlQuery<Int16>($"select max_readers as Value from sys.service_queues where name = 'ArchivePlayerTargetQueue'")
                         .FirstOrDefaultAsync();
                    MaxQueueReadersTextbox.Text = mqr.ToString();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, $"Unable to check service queue configuration");
                    MaxQueueReadersTextbox.Text = "??";
                }
                var pdaStatus = new Dictionary<byte, string>();
                foreach (var i in _pmDataArchivalContext.Statuses)
                {
                    try
                    {
                        pdaStatus.Add((byte)i.Id, i.StatusName);
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, "Unable to build lookup table for PDA statuses");
                    }
                    
                }
                var ptbps = _pmDataArchivalContext.PlayersToBePurgeds
                    .GroupBy(x => x.Processed)
                    .Select(s => new { Processed = s.Key, Count = s.Count() });
           
                playersToBePurgedStatuses.Clear();
                playersToBePurgedStatuses.Add(new PlayersToBePurgedGroupBy() { Count = "123", Status = "testing"});
                foreach (var i in ptbps)
                {

                    playersToBePurgedStatuses.Add(new() { Count = i.Count.ToString(), Status = pdaStatus[i.Processed] });
                    
                }

                var ptbpsc = _pmDataArchivalContext.PlayersToBePurgedCompleteds
                   .GroupBy(x => x.Processed)
                   .Select(s => new { Processed = s.Key, Count = s.Count() });
                playersToBePurgedCompletedStatuses.Clear();
                playersToBePurgedCompletedStatuses.Add(new PlayersToBePurgedGroupBy() { Count = "123", Status = "testing" });
                foreach (var i in ptbpsc)
                {

                    playersToBePurgedCompletedStatuses.Add(new() { Count = i.Count.ToString(), Status = pdaStatus[i.Processed] });

                }
                await UpdateConfig();
                await UpdateJobs();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unable to refresh dashboard");
            }
            finally
            {
                ((Button)sender).IsEnabled = true;
                RefreshDashboardProgressBar.Visibility = Visibility.Hidden;
            }
        }

        private async Task UpdateJobs()
        {
            var jobs = await _playerManagementContext.PmDataArchivalJobReports.FromSql(@$"select name, enabled, sja.start_execution_date, sja.stop_execution_date, sja.next_scheduled_run_date
from msdb.dbo.sysjobs j
outer apply (select top 1 * from msdb.dbo.sysjobactivity sja where sja.job_id = j. job_id
				order by next_scheduled_run_date desc) sja
where name = 'PMDataArchival - Rebuild and Validate players'
	or name = 'PmDataArchival - Queue players for archival'").ToListAsync();

            pmDataArchivalJobReports.Clear();
            foreach (var x in jobs)
            {
                pmDataArchivalJobReports.Add(x);
            }
        }

        private async Task UpdateConfig()
        {
            
            var configItemWithGlobal = await _playerManagementContext.ConfigItems.Include(x => x.ConfigGlobal).Where(x => x.ItemId >= 91001 && x.ItemId <= 91013)
                .ToListAsync();
            dataArchivalConfig.Clear();

            foreach (var item in configItemWithGlobal)
            {
                dataArchivalConfig.Add(new DataArchivalConfigDatagridBindingType()
                {
                    ItemId = item.ItemId.ToString(),
                    Description = item.Description,
                    LongDescription = item.LongDescription,
                    IsDefault = item.ConfigGlobal is null ? true : false, //if configglobal doesn't exist we use the default value
                    ConfigValue = item.ConfigGlobal is null ? item.DefaultValue : item.ConfigGlobal.ConfigValue
                });
            }
        }




        //public async void GetPlayerIdCountClick(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (int.TryParse(GroupIDTextbox.Text, out int groupIdInt))
        //        {
        //            var count = await _playerManagementContext.CasinoGroups.Where(x => x.GroupId == groupIdInt).CountAsync();
        //            PlayerIdCountTextbox.Text = count.ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Unable to get player count");
        //    }


        //}

        //        public async void CheckForIndexesWithDisabledLocks(object sender, RoutedEventArgs e)
        //        {
        //            try
        //            {
        //                var indexissues = await _playerManagementContext.IndexesWithPageAndRowLocksDisabledReturnValues
        //                    .FromSql(@$"SELECT t.name AS TableName
        //	,i.name AS IndexName
        //	,i.allow_page_locks
        //	,i.allow_row_locks
        //	,'ALTER INDEX [' + I.name + '] ON [' + SCHEMA_NAME(T.SCHEMA_ID) + '].[' + T.name + '] REBUILD WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON)' AS FixCommand
        //FROM PlayerManagement.sys.indexes i
        //JOIN PlayerManagement.sys.tables t ON t.object_id = i.object_id
        //WHERE (
        //		allow_row_locks <> 1
        //		OR allow_page_locks <> 1
        //		)
        //	AND t.is_ms_shipped = 0
        //ORDER BY TableName
        //	,IndexName")
        //                    .AsNoTracking()
        //                    .ToListAsync();
        //                if(indexissues.Count == 0)
        //                {
        //                    IndexesWithDisableLogsTextbox.Text = "No indexes with disabled locks found.";
        //                    return;
        //                }

        //                var messageText = "Indexes with disable locks found. Run the following to fix..." + Environment.NewLine;
        //                 foreach (var line in indexissues)
        //                {
        //                    messageText += line.FixCommand + Environment.NewLine;
        //                }
        //                IndexesWithDisableLogsTextbox.Text = messageText;

        //            }
        //            catch(Exception ex)
        //            {
        //                _logger.LogError(ex, "OnClick1 Failure");
        //            }
        //        }




    }
    public class PlayersToBePurgedGroupBy {
        public string? Status { get; set; }
        public string? Count { get; set; }
    }
    public class DataArchivalConfigDatagridBindingType
    {
        public string? ItemId { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public bool IsDefault { get; set; }
        public string? ConfigValue { get; set; }
    }
}