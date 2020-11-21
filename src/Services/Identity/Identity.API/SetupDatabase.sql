IF NOT EXISTS(SELECT 1 FROM sys.databases WHERE name='cmessaging_identity')
    CREATE DATABASE [cmessaging_identity]
    