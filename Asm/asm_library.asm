.data

minusOne DD 0bf800000r ; -1

resultCoeffsOffset = 48

.code

convertRaw PROC                               ; COMDAT
$LN60:
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
        jne     SHORT $LN20@convertRaw
        movss   DWORD PTR [r10], xmm2
        jmp     $LN18@convertRaw
$LN20@convertRaw:
        mov     eax, DWORD PTR [rcx]
        lea     rbx, QWORD PTR [r10+4]
        mov     ecx, 2
        mov     DWORD PTR [r10], eax
        mov     DWORD PTR [rbx], 1065353216             ; 3f800000H
        lea     r9d, QWORD PTR [rcx-1]
        jmp     $LN58@convertRaw
$LL4@convertRaw:
        movss   xmm1, DWORD PTR [r14+r9*4]
        mov     rsi, rcx
        test    rcx, rcx
        je      SHORT $LN6@convertRaw
        mov     r8, r10
        mov     rax, rbp
        sub     r8, rbp
        mov     r11, rcx
$LL36@convertRaw:
        movaps  xmm0, xmm1
        mulss   xmm0, DWORD PTR [r8+rax]
        movss   DWORD PTR [rax], xmm0
        lea     rax, QWORD PTR [rax+4]
        sub     r11, 1
        jne     SHORT $LL36@convertRaw
$LN6@convertRaw:
        add     rbx, 4
        inc     rcx
        mov     r8, rsi
        mov     DWORD PTR [rbx], edx
        cmp     rsi, 1
        jb      SHORT $LN9@convertRaw
$LL10@convertRaw:
        mov     eax, DWORD PTR [r10+r8*4-4]
        mov     DWORD PTR [r10+r8*4], eax
        dec     r8
        cmp     r8, 1
        jae     SHORT $LL10@convertRaw
        mov     r11, rbp
        mov     DWORD PTR [r10], edx
        sub     r11, r10
        mov     r8, rdx
        mov     rax, r10
$LL38@convertRaw:
        movss   xmm0, DWORD PTR [r11+rax]
        inc     r8
        addss   xmm0, DWORD PTR [rax]
        movss   DWORD PTR [rax], xmm0
        add     rax, 4
        cmp     r8, rsi
        jb      SHORT $LL38@convertRaw
        jmp     SHORT $LN2@convertRaw
$LN9@convertRaw:
        mov     DWORD PTR [r10], edx
$LN2@convertRaw:
        inc     r9
$LN58@convertRaw:
        cmp     r9, rdi
        jb      $LL4@convertRaw
        movss   xmm1, DWORD PTR minusOne
        mov     rax, rdx
        test    rcx, rcx
        je      SHORT $LN15@convertRaw
$LL40@convertRaw:
        movaps  xmm0, xmm2
        mulss   xmm0, DWORD PTR [r10+rax*4]
        movss   DWORD PTR [r10+rax*4], xmm0
        test    al, 1
        jne     SHORT $LN41@convertRaw
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rax*4], xmm0
$LN41@convertRaw:
        inc     rax
        cmp     rax, rcx
        jb      SHORT $LL40@convertRaw
$LN15@convertRaw:
        test    cl, 1
        je      SHORT $LN18@convertRaw
        test    rcx, rcx
        je      SHORT $LN18@convertRaw
$LL43@convertRaw:
        movss   xmm0, DWORD PTR [r10+rdx*4]
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rdx*4], xmm0
        inc     rdx
        cmp     rdx, rcx
        jb      SHORT $LL43@convertRaw
$LN18@convertRaw:
        mov     rbx, QWORD PTR [rsp+16]
        mov     rbp, QWORD PTR [rsp+24]
        mov     rsi, QWORD PTR [rsp+32]
        mov     rdi, QWORD PTR [rsp+40]
        pop     r14
        ret     0
convertRaw ENDP

END