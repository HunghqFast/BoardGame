

select top 5 a.stt_rec, IDENTITY(int, 1, 1) as id, a.client_code, a.ma_dvcs, a.ngay_ct, a.user_id0 , b.xml_invoice, a.cks_cqt, a.ngay_ky_cqt, a.cks_cn, a.ngay_ky_cn, a.qrcode_img, 'abc.pdf' as pdf_path
	into #table from m05$202201 a join x05$202201 b on a.stt_rec = b.stt_rec where a.client_code = '908889'

/*
select * into #m from m05$202201 where stt_rec in (select stt_rec from #table)
select * into #d from d05$202201 where stt_rec in (select stt_rec from #table)
select * into #c from c05$000000 where stt_rec in (select stt_rec from #table)
select * into #i from i05$202201 where stt_rec in (select stt_rec from #table)
select * into #x from x05$202201 where stt_rec in (select stt_rec from #table)
select * into #r from r30$202201 where stt_rec in (select stt_rec from #table)

delete m05$202201 where stt_rec in (select stt_rec from #table)
delete d05$202201 where stt_rec in (select stt_rec from #table)
delete c05$000000 where stt_rec in (select stt_rec from #table)
delete i05$202201 where stt_rec in (select stt_rec from #table)
delete x05$202201 where stt_rec in (select stt_rec from #table)
delete r30$202201 where stt_rec in (select stt_rec from #table)


exec rs_AutoCreateListInputInvoiceSchedule '#table', 'xml_invoice', 'client_code', 'ma_dvcs', 'user_id0', 'ngay_ct', 'cks_cqt', 'ngay_ky_cqt', 'cks_cn', 'ngay_ky_cn', 'qrcode_img', 'pdf_path'
*/
drop table #table

/*
insert into m05$202201 select * from #m
insert into d05$202201 select * from #d
insert into c05$000000 select * from #c
insert into i05$202201 select * from #i
insert into x05$202201 select * from #x
insert into r30$202201 select * from #r

drop table #m, #d, #c, #i, #x, #r
*/

-- exec rs_AutoCreateInputInvoiceSchedule '908889', 100

-- select * from xmlqueue; select * from x05$202201 where stt_rec in (select keys from xmlqueue)

-- delete x05$202201 where stt_rec in (select keys from xmlqueue); delete xmlqueue where 1 = 1

-- select * from x05$202201
-- select * from m05$202201
-- select * from i05$202201

-- delete x05$202201 where len(stt_rec) = 32
-- delete x05$202201 where stt_rec not in (select stt_rec from m05$202201)
-- delete x05$202201 where stt_rec = '12345'


-- select * from m05$202201 where stt_rec not in (select stt_rec from x05$202201)
-- delete m05$202201 where stt_rec not in (select stt_rec from x05$202201)
-- delete d05$202201 where stt_rec not in (select stt_rec from m05$202201)

