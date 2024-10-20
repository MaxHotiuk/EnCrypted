using EnCryptedAPI.Data;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;
using EnCryptedAPI.Models.Domain;
using CancellationToken = EnCryptedAPI.Models.Domain.CancellationToken;
using System.Reflection;

namespace EnCryptedAPI.Logic
{
    public static class EncryptionLogic
    {
        public static async System.Threading.Tasks.Task EncryptOrDecryptData(EncryptDataDto request, EnCryptedDbContext context)
        {
            if (!request.DataEncrypted)
            {
                await EncryptTaskData(request.TaskID, context); // Pass the cancellationToken here
            }
            else
            {
                await DecryptTaskData(request.TaskID, context); // Pass the cancellationToken here
            }
        }

        public static async System.Threading.Tasks.Task EncryptTaskData(Guid taskId, EnCryptedDbContext context)
        {
            var task = await context.Tasks.Include(t => t.EncryptionJobs).FirstOrDefaultAsync(t => t.TaskID == taskId);
            if (task == null)
            {
                return;
            }

            var cancellationEntry = await context.CancellationTokens.FirstOrDefaultAsync(ct => ct.TaskID == taskId);

            var totalWords = task.EncryptionJobs.Count;
            var wordsEncrypted = 0;

            foreach (var job in task.EncryptionJobs)
            {
                // Force reloading of cancellation token and task status to reflect any external updates
                if (cancellationEntry != null)
                {
                    await context.Entry(cancellationEntry).ReloadAsync();
                }
                await context.Entry(task).ReloadAsync();

                // Check for cancellation or task completion at the start of each job
                if (cancellationEntry == null || cancellationEntry.IsCanceled || task.IsCompleted)
                {
                    throw new OperationCanceledException("Task was canceled or completed.");
                }

                var encryptedData = job.EncryptedData != null 
                    ? Encryption.Encryption.Encrypt(job.EncryptedData, job.PassPhrase) // Encrypt the data
                    : null;

                job.EncryptedData = encryptedData;
                job.DataEncrypted = true;
                wordsEncrypted++;
                task.Progress = (int)((double)wordsEncrypted / totalWords * 100);

                await context.SaveChangesAsync(); // Save progress after each job
            }
        }


        public static async System.Threading.Tasks.Task DecryptTaskData(Guid taskId, EnCryptedDbContext context)
        {
            var task = await context.Tasks.Include(t => t.EncryptionJobs).FirstOrDefaultAsync(t => t.TaskID == taskId);
            if (task == null)
            {
                return;
            }

            var totalWords = task.EncryptionJobs.Count;
            var wordsDecrypted = 0;

            foreach (var job in task.EncryptionJobs)
            {
                var tokencancel = await context.CancellationTokens.FirstOrDefaultAsync(ct => ct.TaskID == taskId);
                if (tokencancel != null && tokencancel.IsCanceled) // Check for cancellation at the start of each job
                {
                    throw new OperationCanceledException();
                }

                var decryptedData = job.EncryptedData != null 
                    ? Encryption.Encryption.Decrypt(job.EncryptedData, job.PassPhrase) // Decrypt the data
                    : null;

                job.EncryptedData = decryptedData;
                job.DataEncrypted = false;
                wordsDecrypted++;
                task.Progress = (int)((double)wordsDecrypted / totalWords * 100);

                await context.SaveChangesAsync(); // Save progress after each job

                // Update cancellation token in the database
                var cancellationEntry = await context.CancellationTokens.FirstOrDefaultAsync(ct => ct.TaskID == taskId);
                if (cancellationEntry != null && cancellationEntry.IsCanceled)
                {
                    throw new OperationCanceledException();
                }
            }
        }
    }
}
