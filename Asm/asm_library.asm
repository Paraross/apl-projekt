.data

minusOne DD 0bf800000r ; -1.0

resultCoeffsOffset = 48 ; offset from the stack frame

.code

; Parameters are passed according to the x64 calling convention:
; rcx         : float* roots
; xmm1        : float scale
; r8          : long len
; r9          : float* resultCoeffsPrev
; Stack (rsp) : float* resultCoeffs
; Registers saved/restored: rbx, rbp, rsi, rdi, r14
Convert PROC
        ; save nonvolatile registers
        mov     rax, rsp
        mov     QWORD PTR [rax+8], rbx
        mov     QWORD PTR [rax+16], rbp
        mov     QWORD PTR [rax+24], rsi
        mov     QWORD PTR [rax+32], rdi
        push    r14
        mov     r10, QWORD PTR resultCoeffsOffset[rsp]
        xor     edx, edx
        mov     rbp, r9
        mov     rdi, r8
        movaps  xmm2, xmm1
        mov     r14, rcx
        test    r8, r8
        ; return early if len == 0
        jne     SHORT ActualFunctionStart
        movss   DWORD PTR [r10], xmm2
        jmp     Return
ActualFunctionStart:
        ; initialize resultCoeffs
        mov     eax, DWORD PTR [rcx]
        lea     rbx, QWORD PTR [r10+4]
        ; ecx = resultCoeffsLen
        mov     ecx, 2
        mov     DWORD PTR [r10], eax
        mov     DWORD PTR [rbx], 03f800000h ; 1.0
        lea     r9d, QWORD PTR [rcx-1]
        jmp     OuterLoop2
OuterLoop1: ; process each root from roots
        movss   xmm1, DWORD PTR [r14+r9*4]
        mov     rsi, rcx
        test    rcx, rcx
        ; skip multiplication by root if resultCoeffsLen == 0
        je      SHORT AfterInnerLoop1
        mov     r8, r10
        mov     rax, rbp
        sub     r8, rbp
        mov     r11, rcx
InnerLoop1: ; multiply each element of resultCoeffsPrev by root
        movaps  xmm0, xmm1
        mulss   xmm0, DWORD PTR [r8+rax]
        movss   DWORD PTR [rax], xmm0
        lea     rax, QWORD PTR [rax+4]
        sub     r11, 1
        jne     SHORT InnerLoop1
AfterInnerLoop1:
        add     rbx, 4
        inc     rcx
        mov     r8, rsi
        mov     DWORD PTR [rbx], edx
        cmp     rsi, 1
        ; skip shifting if resultCoeffsLen == 1
        jb      SHORT AfterInnerLoop2
InnerLoop2: ; shift resultCoeffs elements by one place to the right
        mov     eax, DWORD PTR [r10+r8*4-4]
        mov     DWORD PTR [r10+r8*4], eax
        dec     r8
        cmp     r8, 1
        jae     SHORT InnerLoop2
        mov     r11, rbp
        mov     DWORD PTR [r10], edx
        sub     r11, r10
        mov     r8, rdx
        mov     rax, r10
InnerLoop3: ; add resultCoeffsPrev to resultCoeffs
        movss   xmm0, DWORD PTR [r11+rax]
        inc     r8
        addss   xmm0, DWORD PTR [rax]
        movss   DWORD PTR [rax], xmm0
        add     rax, 4
        cmp     r8, rsi
        jb      SHORT InnerLoop3
        jmp     SHORT IncrementLoopCounter
AfterInnerLoop2:
        ; resultCoeffs[0] = 0.0
        mov     DWORD PTR [r10], edx
IncrementLoopCounter:
        inc     r9
OuterLoop2: ; multiply each resultCoeffs element by scale, and each element on an even index by -1.0
        cmp     r9, rdi
        jb      OuterLoop1
        movss   xmm1, DWORD PTR minusOne
        mov     rax, rdx
        test    rcx, rcx
        je      SHORT AfterOuterLoop2
OuterLoop2MulScale:
        movaps  xmm0, xmm2
        ; multiply each element by scale
        mulss   xmm0, DWORD PTR [r10+rax*4]
        movss   DWORD PTR [r10+rax*4], xmm0
        test    al, 1
        jne     SHORT OuterLoop2End
        ; multiply each even element by -1.0
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rax*4], xmm0
OuterLoop2End:
        inc     rax
        cmp     rax, rcx
        jb      SHORT OuterLoop2MulScale
AfterOuterLoop2: ; return if resultCoeffsLen is even
        test    cl, 1
        je      SHORT Return
        test    rcx, rcx
        je      SHORT Return
OuterLoop3: ; multiply each resultCoeffs element by -1.0
        movss   xmm0, DWORD PTR [r10+rdx*4]
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rdx*4], xmm0
        inc     rdx
        cmp     rdx, rcx
        jb      SHORT OuterLoop3
Return: ; restore nonvolatile registers and return
        mov     rbx, QWORD PTR [rsp+16]
        mov     rbp, QWORD PTR [rsp+24]
        mov     rsi, QWORD PTR [rsp+32]
        mov     rdi, QWORD PTR [rsp+40]
        pop     r14
        ret     0
Convert ENDP

END