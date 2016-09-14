Tests another basic scenario:

RootOrderedTest => [Stop on error]
	Test

	OrderedTest => [Continue on error]
		Test
		Test
		Test

		OrderedTest => [Stop on error]
			Test
			Test
			Test **FAIL**
			Test
		
		Test

	Test
	Test
	Test