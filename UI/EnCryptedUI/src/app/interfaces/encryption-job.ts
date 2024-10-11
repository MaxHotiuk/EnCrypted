export interface EncryptionJob {
  id: string;               // Unique identifier for the encryption job
  userID: string;           // ID of the user who owns the job
  taskID: string;           // ID of the related task
  dataEncrypted: boolean;   // Whether the data is encrypted or not
  encryptedData: string;    // The actual encrypted data
  createdAt: Date;          // Timestamp when the job was created
  passPhrase: string;       // Passphrase used for encryption
}
