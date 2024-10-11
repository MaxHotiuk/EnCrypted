using EnCryptedAPI.Data;
using EnCryptedAPI.Requests;
using Microsoft.EntityFrameworkCore;

namespace EnCryptedAPI.Logic;

public static class EncryptionLogic
{
    public static async System.Threading.Tasks.Task EncryptOrDecryptData(EncryptDataDto request, EnCryptedDbContext context)
    {
        if (!request.DataEncrypted)
        {
            await EncryptTaskData(request.TaskID, context);
        }
        else
        {
            await DecryptTaskData(request.TaskID, context);
        }
    }
    public static async System.Threading.Tasks.Task EncryptTaskData(Guid taskId, EnCryptedDbContext context)
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
            var encryptedData = job.EncryptedData != null ? Encryption.Encryption.Encrypt(job.EncryptedData, job.PassPhrase) : null;
            job.EncryptedData = encryptedData;
            job.DataEncrypted = true;
            wordsEncrypted++;
            task.Progress = (int)((double)wordsEncrypted / totalWords * 100);
            await context.SaveChangesAsync();
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
            var decryptedData = job.EncryptedData != null ? Encryption.Encryption.Decrypt(job.EncryptedData, job.PassPhrase) : null;
            job.EncryptedData = decryptedData;
            job.DataEncrypted = false;
            wordsDecrypted++;
            task.Progress = (int)((double)wordsDecrypted / totalWords * 100);
        }

        await context.SaveChangesAsync();
    }
}