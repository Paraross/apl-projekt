.data

__real@bf800000 DD 0bf800000r             ; -1

roots$ = 112
scale$ = 120
len$ = 128

.code

EXTERN malloc : PROC
EXTERN free : PROC

convert_raw PROC                                        ; COMDAT
$LN59:
        mov     rax, rsp
        mov     QWORD PTR [rax+8], rbx
        mov     QWORD PTR [rax+16], rbp
        mov     QWORD PTR [rax+24], rsi
        push    rdi
        push    r12
        push    r13
        push    r14
        push    r15
        sub     rsp, 64                             ; 00000040H
        xor     ebx, ebx
        movaps  XMMWORD PTR [rax-56], xmm6
        movaps  XMMWORD PTR [rax-72], xmm7
        mov     rbp, r8
        movaps  xmm7, xmm1
        mov     r13, rcx
        test    r8, r8
        jne     SHORT $LN20@convert_ra
        xor     eax, eax
        jmp     $LN1@convert_ra
$LN20@convert_ra:
        lea     rcx, QWORD PTR [r8*4+4]
        call    malloc
        mov     rdi, rax
        mov     eax, DWORD PTR [r13]
        mov     esi, 2
        mov     r15, rbx
        mov     DWORD PTR [rdi], eax
        mov     DWORD PTR [rdi+4], 1065353216       ; 3f800000H
        test    rbp, rbp
        je      $LN3@convert_ra
        lea     r14d, QWORD PTR [rsi+6]
$LL4@convert_ra:
        movss   xmm6, DWORD PTR [r13+r15*4]
        mov     rcx, r14
        mov     r12, rsi
        call    malloc
        mov     r8, rax
        test    rsi, rsi
        je      SHORT $LN6@convert_ra
        mov     rcx, rdi
        mov     rdx, rsi
        sub     rcx, rax
$LL36@convert_ra:
        movaps  xmm0, xmm6
        mulss   xmm0, DWORD PTR [rcx+rax]
        movss   DWORD PTR [rax], xmm0
        lea     rax, QWORD PTR [rax+4]
        sub     rdx, 1
        jne     SHORT $LL36@convert_ra
$LN6@convert_ra:
        inc     rsi
        mov     rcx, r12
        lea     r14, QWORD PTR [rsi*4]
        mov     DWORD PTR [rdi+r14-4], ebx
        cmp     r12, 1
        jb      SHORT $LN9@convert_ra
$LL10@convert_ra:
        mov     eax, DWORD PTR [rdi+rcx*4-4]
        mov     DWORD PTR [rdi+rcx*4], eax
        dec     rcx
        cmp     rcx, 1
        jae     SHORT $LL10@convert_ra
        mov     rdx, r8
        mov     DWORD PTR [rdi], ebx
        sub     rdx, rdi
        mov     rcx, rbx
        mov     rax, rdi
$LL38@convert_ra:
        movss   xmm0, DWORD PTR [rdx+rax]
        inc     rcx
        addss   xmm0, DWORD PTR [rax]
        movss   DWORD PTR [rax], xmm0
        add     rax, 4
        cmp     rcx, r12
        jb      SHORT $LL38@convert_ra
        jmp     SHORT $LN12@convert_ra
$LN9@convert_ra:
        mov     DWORD PTR [rdi], ebx
$LN12@convert_ra:
        mov     rcx, r8
        call    free
        inc     r15
        cmp     r15, rbp
        jb      $LL4@convert_ra
$LN3@convert_ra:
        movss   xmm1, DWORD PTR __real@bf800000
        mov     rax, rbx
        test    rsi, rsi
        je      SHORT $LN15@convert_ra
$LL40@convert_ra:
        movaps  xmm0, xmm7
        mulss   xmm0, DWORD PTR [rdi+rax*4]
        movss   DWORD PTR [rdi+rax*4], xmm0
        test    al, 1
        jne     SHORT $LN41@convert_ra
        mulss   xmm0, xmm1
        movss   DWORD PTR [rdi+rax*4], xmm0
$LN41@convert_ra:
        inc     rax
        cmp     rax, rsi
        jb      SHORT $LL40@convert_ra
$LN15@convert_ra:
        lea     eax, DWORD PTR [rsi+255]
        test    al, 1
        jne     SHORT $LN18@convert_ra
        test    rsi, rsi
        je      SHORT $LN18@convert_ra
$LL43@convert_ra:
        movss   xmm0, DWORD PTR [rdi+rbx*4]
        mulss   xmm0, xmm1
        movss   DWORD PTR [rdi+rbx*4], xmm0
        inc     rbx
        cmp     rbx, rsi
        jb      SHORT $LL43@convert_ra
$LN18@convert_ra:
        mov     rax, rdi
$LN1@convert_ra:
        movaps  xmm6, XMMWORD PTR [rsp+48]
        lea     r11, QWORD PTR [rsp+64]
        mov     rbx, QWORD PTR [r11+48]
        mov     rbp, QWORD PTR [r11+56]
        mov     rsi, QWORD PTR [r11+64]
        movaps  xmm7, XMMWORD PTR [rsp+32]
        mov     rsp, r11
        pop     r15
        pop     r14
        pop     r13
        pop     r12
        pop     rdi
        ret     0
convert_raw ENDP

END