# Moneybox Money Withdrawal

The solution contains a .NET core library (Moneybox.App) which is structured into the following 3 folders:

* Domain - this contains the domain models for a user and an account, and a notification service.
* Features - this contains two operations, one which is implemented (transfer money) and another which isn't (withdraw money)
* DataAccess - this contains a repository for retrieving and saving an account (and the nested user it belongs to)

## The task

The task is to implement a money withdrawal in the WithdrawMoney.Execute(...) method in the features folder. For consistency, the logic should be the same as the TransferMoney.Execute(...) method i.e. notifications for low funds and exceptions where the operation is not possible. 

As part of this process however, you should look to refactor some of the code in the TransferMoney.Execute(...) method into the domain models, and make these models less susceptible to misuse. We're looking to make our domain models rich in behaviour and much more than just plain old objects, however we don't want any data persistance operations (i.e. data access repositories) to bleed into our domain. This should simplify the task of implementing WithdrawMoney.Execute(...).

## Guidelines

* The test should take about an hour to complete, although there is no strict time limit
* You should fork or copy this repository into your own public repository (Github, BitBucket etc.) before you do your work
* Your solution must build and any tests must pass
* You should not alter the notification service or the the account repository interfaces
* You may add unit/integration tests using a test framework (and/or mocking framework) of your choice
* You may edit this README.md if you want to give more details around your work (e.g. why you have done something a particular way, or anything else you would look to do but didn't have time)
* A reasonable use of AI is permitted, but we want to see your own work and understanding reflected in the solution

Once you have completed test, zip up your solution, excluding any build artifacts to reduce the size, and email it back to our recruitment team.

Good luck!

--------------------------------------------------------------------------------------------------

# Implementation notes:

## Accout class improvements:
I have refactored the Account class to include methods for withdrawing and transferring money, 
which encapsulates the logic for these operations and makes the domain model richer in behavior.

* Encapsulation: Business rules moved into methods.
* Validation: Added checks for sufficient funds and valid transfer amounts.
* Constants: Used constants to avoid magic numbers.
* Helper methods: Created helper methods for better visibility, reuse, and cleaner code.

## Transfer/Withdraw Money improvements:
* Refactored the .Execute method to utilize the new methods in the Account class.
* Elimiated code duplication by centralizing the logic for checking funds and sending notifications.
* Transaction safety `TransactionScope` for transfers: Ensured that both accounts are updated atomically to prevent inconsistencies.
* Notifcations are triggered after transactions are successful, preventing unwanted notifications in case of failures.

## Tests:
* Added unit tests for the new methods in the Account class to ensure they behave correctly under various scenarios.
* Added tests for the .Execute method (Withdraw/Transfer) to verify that it correctly handles withdrawals and sends notifications

# If I had more time:
* I would add more comprehensive tests, including edge cases, bringing the coverage up to 80-90%.
* I would refactor public setters on the `Account` class, enforce validity at construction, and introduce domain-specific exceptions / result types instead of generic exceptions.
* Improve transactional and concurrency behavior for transfers by abstracting transaction management and adding tests that assert all-or-nothing updates under failure scenarios.
* Add basic observability (logging/metrics) around withdraw/transfer flows and introduce static analysis rules in CI to maintain code quality over time.
* Introduce async/await across the application boundary (e.g. `IAccountRepository` and the `TransferMoney` / `WithdrawMoney` use cases) so that the operations are non-blocking.