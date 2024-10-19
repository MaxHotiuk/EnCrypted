using EnCryptedAPI.Data;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;

namespace EnCryptedAPI.Logic;

public static class EncryptionLogic
{
    public static async System.Threading.Tasks.Task EncryptOrDecryptData(EncryptDataDto request, EnCryptedDbContext context, CancellationToken cancellationToken)
    {
        if (!request.DataEncrypted)
        {
            await EncryptTaskData(request.TaskID, context, cancellationToken); // Pass the cancellationToken here
        }
        else
        {
            await DecryptTaskData(request.TaskID, context, cancellationToken); // Pass the cancellationToken here
        }
    }

    public static async System.Threading.Tasks.Task EncryptTaskData(Guid taskId, EnCryptedDbContext context, CancellationToken cancellationToken)
    {
        var task = await context.Tasks.Include(t => t.EncryptionJobs).FirstOrDefaultAsync(t => t.TaskID == taskId);
        if (task == null)
        {
            return;
        }

        var totalWords = task.EncryptionJobs.Count;
        var wordsEncrypted = 0;
        
        foreach (var job in task.EncryptionJobs)
        {
            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation at the start of each job

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
    public static async System.Threading.Tasks.Task DecryptTaskData(Guid taskId, EnCryptedDbContext context, CancellationToken cancellationToken)
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
            cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation at the start of each job

            var decryptedData = job.EncryptedData != null 
                ? Encryption.Encryption.Decrypt(job.EncryptedData, job.PassPhrase) // Decrypt the data
                : null;

            job.EncryptedData = decryptedData;
            job.DataEncrypted = false;
            wordsDecrypted++;
            task.Progress = (int)((double)wordsDecrypted / totalWords * 100);
            
            await context.SaveChangesAsync(); // Save progress after each job
        }
    }

}