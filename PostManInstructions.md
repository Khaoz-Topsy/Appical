### Using the PostMan collection

- Import the Collection
- Import the Environment Variables
	- apiUrl: https://appical.kurtlourens.com
	- useCase2AmountToAdd: 300
	- useCase3AmountToSubtract: -100
	- useCase3AmountToGetToZeroBalance: -200

UseCase1
- Step 1, copy `id` and paste in Environment Variable `accountOwnerGuid`
- Step 2, copy `id` and paste in Environment Variable `accountGuid`

UseCase2
- Step 1, copy `id` and paste in Environment Variable `latestTransaction`
- Step 2
- Step 3, confirm balance is equal to EnvironmentVariable `useCase2AmountToAdd`

UseCase3
- Step 1, copy `id` and paste in Environment Variable `latestTransaction`
- Step 2, Should fail. Balance cennot be negative
- Step 3
- Step 4, Balance should be equal to `useCase2AmountToAdd` - `useCase3AmountToSubtract`

UseCase4
- Step 1, copy `id` and paste in Environment Variable `latestTransaction`
- Step 2, Should fail. Cannot close account if balance is not zero
- Step 3
- Step 4, Balance should be 0
- Step 5, Should return closed account with Closure reason, and date

UseCase5
- Step 1, should display accounts

UseCase6
- Step 1, should display accounts





