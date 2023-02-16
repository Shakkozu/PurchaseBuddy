Feature: Feature1

Management of user shops should be possible
Management includes basic CRUD operations


@tag1
Scenario: Add shop
	Given the user shops list is empty
	When the user is registered
	When the user adds a shop
	Then the shop is added to the list
