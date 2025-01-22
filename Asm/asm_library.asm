.data

__real@bf800000 DD 0bf800000r             ; -1

roots$ = 16
scale$ = 24
len$ = 32
result_coeffs_prev$ = 40
result_coeffs$ = 48

.code

convertRaw PROC                                        ; COMDAT
$LN58:
        mov     rax, rsp
        mov     QWORD PTR [rax+8], rbx
        mov     QWORD PTR [rax+16], rbp
        mov     QWORD PTR [rax+24], rsi
        mov     QWORD PTR [rax+32], rdi
        push    r14
        mov     r11, QWORD PTR result_coeffs$[rsp]
        xor     edx, edx
        mov     eax, DWORD PTR [rcx] ; chuj
        mov     rsi, r9
        mov     rbp, r8
        movaps  xmm2, xmm1
        mov     r14, rcx
        mov     r9d, edx
        mov     DWORD PTR [r11], eax
        lea     rdi, QWORD PTR [r11+4]
        mov     DWORD PTR [rdi], 1065353216             ; 3f800000H
        lea     r10d, QWORD PTR [rdx+2]
        test    r8, r8
        je      $LN3@convert_ra
$LL4@convert_ra:
        movss   xmm1, DWORD PTR [r14+r9*4]
        mov     rbx, r10
        test    r10, r10
        je      SHORT $LN6@convert_ra
        mov     rcx, r11
        mov     rax, rsi
        sub     rcx, rsi
        mov     r8, r10
$LL35@convert_ra:
        movaps  xmm0, xmm1
        mulss   xmm0, DWORD PTR [rcx+rax]
        movss   DWORD PTR [rax], xmm0
        lea     rax, QWORD PTR [rax+4]
        sub     r8, 1
        jne     SHORT $LL35@convert_ra
$LN6@convert_ra:
        add     rdi, 4
        inc     r10
        mov     rcx, rbx
        mov     DWORD PTR [rdi], edx
        cmp     rbx, 1
        jb      SHORT $LN9@convert_ra
$LL10@convert_ra:
        mov     eax, DWORD PTR [r11+rcx*4-4]
        mov     DWORD PTR [r11+rcx*4], eax
        dec     rcx
        cmp     rcx, 1
        jae     SHORT $LL10@convert_ra
        mov     r8, rsi
        mov     DWORD PTR [r11], edx
        sub     r8, r11
        mov     rcx, rdx
        mov     rax, r11
$LL37@convert_ra:
        movss   xmm0, DWORD PTR [r8+rax]
        inc     rcx
        addss   xmm0, DWORD PTR [rax]
        movss   DWORD PTR [rax], xmm0
        add     rax, 4
        cmp     rcx, rbx
        jb      SHORT $LL37@convert_ra
        jmp     SHORT $LN2@convert_ra
$LN9@convert_ra:
        mov     DWORD PTR [r11], edx
$LN2@convert_ra:
        inc     r9
        cmp     r9, rbp
        jb      $LL4@convert_ra
$LN3@convert_ra:
        movss   xmm1, DWORD PTR __real@bf800000
        mov     rax, rdx
        test    r10, r10
        je      SHORT $LN15@convert_ra
$LL39@convert_ra:
        movaps  xmm0, xmm2
        mulss   xmm0, DWORD PTR [r11+rax*4]
        movss   DWORD PTR [r11+rax*4], xmm0
        test    al, 1
        jne     SHORT $LN40@convert_ra
        mulss   xmm0, xmm1
        movss   DWORD PTR [r11+rax*4], xmm0
$LN40@convert_ra:
        inc     rax
        cmp     rax, r10
        jb      SHORT $LL39@convert_ra
$LN15@convert_ra:
        test    r10b, 1
        je      SHORT $LN18@convert_ra
        test    r10, r10
        je      SHORT $LN18@convert_ra
$LL42@convert_ra:
        movss   xmm0, DWORD PTR [r11+rdx*4]
        mulss   xmm0, xmm1
        movss   DWORD PTR [r11+rdx*4], xmm0
        inc     rdx
        cmp     rdx, r10
        jb      SHORT $LL42@convert_ra
$LN18@convert_ra:
        mov     rbx, QWORD PTR [rsp+16]
        mov     rbp, QWORD PTR [rsp+24]
        mov     rsi, QWORD PTR [rsp+32]
        mov     rdi, QWORD PTR [rsp+40]
        pop     r14
        ret     0
convertRaw ENDP

END