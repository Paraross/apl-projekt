.data

.code

AsmAddTwoDoubles PROC

	vaddpd ymm0, ymm0, ymm1
	ret

AsmAddTwoDoubles ENDP

end
