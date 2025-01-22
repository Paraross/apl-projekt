.data

__real@bf800000 DD 0bf800000r             ; -1

roots$ = 16
scale$ = 24
len$ = 32
result_coeffs_prev$ = 40
result_coeffs$ = 48

.code

convertRaw PROC                                        ; COMDAT
$LN61:
        test    r8, r8
        je      $LN58@convert_ra
        mov     rax, rsp
        mov     QWORD PTR [rax+8], rbx
        mov     QWORD PTR [rax+16], rbp
        mov     QWORD PTR [rax+24], rsi
        mov     QWORD PTR [rax+32], rdi
        push    r14
        mov     eax, DWORD PTR [rcx]
        xor     edx, edx
        mov     r10, QWORD PTR result_coeffs$[rsp]
        mov     rbp, r9
        mov     r14, rcx
        mov     rdi, r8
        movaps  xmm2, xmm1
        lea     ecx, QWORD PTR [rdx+2]
        lea     rbx, QWORD PTR [r10+4]
        mov     DWORD PTR [r10], eax
        mov     DWORD PTR [rbx], 1065353216             ; 3f800000H
        lea     r9d, QWORD PTR [rdx+1]
        jmp     $LN59@convert_ra
$LL4@convert_ra:
        movss   xmm1, DWORD PTR [r14+r9*4]
        mov     rsi, rcx
        test    rcx, rcx
        je      SHORT $LN6@convert_ra
        mov     r8, r10
        mov     rax, rbp
        sub     r8, rbp
        mov     r11, rcx
$LL36@convert_ra:
        movaps  xmm0, xmm1
        mulss   xmm0, DWORD PTR [r8+rax]
        movss   DWORD PTR [rax], xmm0
        lea     rax, QWORD PTR [rax+4]
        sub     r11, 1
        jne     SHORT $LL36@convert_ra
$LN6@convert_ra:
        add     rbx, 4
        inc     rcx
        mov     r8, rsi
        mov     DWORD PTR [rbx], edx
        cmp     rsi, 1
        jb      SHORT $LN9@convert_ra
$LL10@convert_ra:
        mov     eax, DWORD PTR [r10+r8*4-4]
        mov     DWORD PTR [r10+r8*4], eax
        dec     r8
        cmp     r8, 1
        jae     SHORT $LL10@convert_ra
        mov     r11, rbp
        mov     DWORD PTR [r10], edx
        sub     r11, r10
        mov     r8, rdx
        mov     rax, r10
$LL38@convert_ra:
        movss   xmm0, DWORD PTR [r11+rax]
        inc     r8
        addss   xmm0, DWORD PTR [rax]
        movss   DWORD PTR [rax], xmm0
        add     rax, 4
        cmp     r8, rsi
        jb      SHORT $LL38@convert_ra
        jmp     SHORT $LN2@convert_ra
$LN9@convert_ra:
        mov     DWORD PTR [r10], edx
$LN2@convert_ra:
        inc     r9
$LN59@convert_ra:
        cmp     r9, rdi
        jb      $LL4@convert_ra
        movss   xmm1, DWORD PTR __real@bf800000
        mov     rax, rdx
        test    rcx, rcx
        je      SHORT $LN15@convert_ra
$LL40@convert_ra:
        movaps  xmm0, xmm2
        mulss   xmm0, DWORD PTR [r10+rax*4]
        movss   DWORD PTR [r10+rax*4], xmm0
        test    al, 1
        jne     SHORT $LN41@convert_ra
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rax*4], xmm0
$LN41@convert_ra:
        inc     rax
        cmp     rax, rcx
        jb      SHORT $LL40@convert_ra
$LN15@convert_ra:
        test    cl, 1
        je      SHORT $LN18@convert_ra
        test    rcx, rcx
        je      SHORT $LN18@convert_ra
$LL43@convert_ra:
        movss   xmm0, DWORD PTR [r10+rdx*4]
        mulss   xmm0, xmm1
        movss   DWORD PTR [r10+rdx*4], xmm0
        inc     rdx
        cmp     rdx, rcx
        jb      SHORT $LL43@convert_ra
$LN18@convert_ra:
        mov     rbx, QWORD PTR [rsp+16]
        mov     rbp, QWORD PTR [rsp+24]
        mov     rsi, QWORD PTR [rsp+32]
        mov     rdi, QWORD PTR [rsp+40]
        pop     r14
$LN58@convert_ra:
        ret     0
convertRaw ENDP

END