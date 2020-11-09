Feature: BookingManagerBVT
	BVT tests for occupied date. The occupied dates in the fake have a start date 10 days away from now
	And an end date 20 days away from now (end date not inclusive).

@mytag
Scenario Outline: Booking within an occupied period
	Given the start date is <startDays> from now
	And the end date is <endDays> from now
	When the booking is created
	Then the booking should return "<bookingResult>"

	Examples:
	| startDays | endDays | bookingResult |
	| 20        | 24      | true          |
	| 1         | 9       | true          |
	| 10        | 18      | false         |
	| 8			| 20      | false         |
	| 11        | 19      | false         |
	| 8         | 10      | false         |
	| 8         | 19      | false         |
	| 10        | 23      | false         |
	| 19        | 23      | false         |