using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace Program1
{
    class Program
    {
        static void Main()
        {
            ReadingFile readingFile = new ReadingFile();
            ParsingFile parsingFile = new ParsingFile(readingFile.GetFile());
            VorkingOperations vorkingOperations = new VorkingOperations(parsingFile.GetBankOperations(), new Balance(Convert.ToInt32(readingFile.GetFile()[0])));
        }
    }


    class Balance
    {
        int balance { get; set; }
        public Balance(int balanc)
        {
            balance = balanc;
        }

        public int GetBalance()
        {
            return balance;
        }

        public static Balance operator +(int a, Balance b)
        {
            return new Balance(a + b.GetBalance());
        }
        public static Balance operator -(int a, Balance b)
        {
            return new Balance(a - b.GetBalance());
        }
    }

    class VorkingOperations
    {
        List<BankOperation> OperationsList = new List<BankOperation>();
        Balance balance;
        public VorkingOperations(List<BankOperation> list, Balance balance1)
        {
            balance = balance1;
            OperationsAdd(list);
            SortOperation();
            Console.WriteLine(PerformingOperation());
        }

        private void OperationsAdd(List<BankOperation> operation)
        {
            for (int i = 0; i < operation.Count; i++)
            {
                if (operation[i] is Revert && OperationsList != null)
                {
                    OperationsList.RemoveAt(OperationsList.Count - 1);
                }
                else
                {
                    OperationsList.Add(operation[i]);
                }
            }
        }

        private void SortOperation()
        {
            List<BankOperation> operations = OperationsList.OrderBy(item => item.TransferDate).ToList();
        }

        public List<BankOperation> CutBefore(DateTime date)
        {
            return OperationsList.Where((item => item.TransferDate < date)).ToList();
        }

        private int PerformingOperation(List<BankOperation> bankOperations = null)
        {
            List<BankOperation> thisBankOperation;
            if (bankOperations != null)
            {
                thisBankOperation = bankOperations;
            }
            else
            {
                thisBankOperation = OperationsList;
            }
            for (int i = 1; i < thisBankOperation.Count; i++)
            {
                balance = new Balance(thisBankOperation[i].PerformingOperations(balance.GetBalance()));
            }
            if (balance.GetBalance() < 0)
            {
                throw new Exception("Неправильные входные данные");
            }
            return balance.GetBalance();
        }
    }

    abstract class BankOperation
    {
        public int TransferAmount { get; set; }
        public DateTime TransferDate { get; set; }

        public abstract int PerformingOperations(int balance);

        public void BankOPeration(int Amount, DateTime Date)
        {
            TransferAmount = Amount;
            TransferDate = Date;
        }
    }

    class In : BankOperation
    {
        public In(DateTime Date, int Amount)
        {
            TransferAmount = Amount;
            TransferDate = Date;
        }

        public override int PerformingOperations(int balance)
        {
            return balance + TransferAmount;
        }
    }

    class Out : BankOperation
    {
        public Out(DateTime Date, int Amount)
        {
            TransferAmount = Amount;
            TransferDate = Date;
        }

        public override int PerformingOperations(int balance)
        {
            return balance - TransferAmount;
        }
    }

    class Revert : BankOperation
    {
        public Revert(DateTime Date, int Amount = 0)
        {
            TransferAmount = Amount;
            TransferDate = Date;
        }

        public override int PerformingOperations(int balance)
        {
            throw new NotImplementedException();
        }
    }

    class ReadingFile
    {
        readonly string Path = @"../../../test.txt";

        private List<string> File = new List<string>();

        public List<string> GetFile()
        {
            return File;
        }

        public ReadingFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        File.Add(line);
                    }
                }
            }
            catch
            { }
        }
    }

    class ParsingFile
    {
        List<BankOperation> bankOperation = new List<BankOperation>();
        public ParsingFile(List<string> File)
        {
            for (int i = 0; i < File.Count; i++)
            {
                var a = File[i].Split("|");
                if (a[a.Length - 1] == " revert")
                {
                    bankOperation.Add(new Revert(Convert.ToDateTime(a[0]), 0));
                }
                if (a[a.Length - 1] == " in")
                {
                    bankOperation.Add(new In(Convert.ToDateTime(a[0]), Convert.ToInt32(a[1])));
                }
                if (a[a.Length - 1] == " out")
                {
                    bankOperation.Add(new Out(Convert.ToDateTime(a[0]), Convert.ToInt32(a[1])));
                }
            }
        }

        public List<BankOperation> GetBankOperations()
        {
            return bankOperation;
        }
    }
}