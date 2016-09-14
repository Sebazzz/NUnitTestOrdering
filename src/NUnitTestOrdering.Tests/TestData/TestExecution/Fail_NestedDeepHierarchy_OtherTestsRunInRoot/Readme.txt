Tests another basic scenario:

RootOrderedTest => [Continue on error]
	Test

	OrderedTest => [Stop on error]
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