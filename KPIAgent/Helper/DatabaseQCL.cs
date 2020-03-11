using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace KPIAgent.Controller
{
    class DatabaseQCL
    {
        Logger _logger = new Logger();
        public bool SaveTransaction(string dagbknr, string debitGL, string creditGL, double amount, double quantityDebit, double quantityCredit, string description, string reference, string currency, string transactionDate, string employee, string empFullname, string bcode, SqlCommand sqlcmd)
        {
            _logger.Info("Dagbknr : " + dagbknr + " - DebitGL : " + debitGL + " - CreditGL : " + creditGL + " - Amount : " + amount + " - QuantityDebit : " + quantityDebit + " - QuantityCredit : " + quantityCredit + " - Description : " + description + " - Reference : " + reference + " - Currency : " + currency + " - Transaction Date : " + transactionDate + " - EmployeeID : " + employee + " - EmployeeName : " + empFullname + " - Branchcode : " + bcode);

            int empcode = Convert.ToInt32(employee.Substring(4));

            string stringN = "N";
            string stringNull = "NULL";
            string stringZero = "0";
            int intZero = 0;
            double doubleZero = 0.0;

            dagbknr = dagbknr.Trim();

            double qty1 = quantityDebit == 0.0 ? quantityCredit : quantityDebit;

            try
            {
                sqlcmd.CommandText = "select min(number) from numbers where used = 0";
                sqlcmd.CommandType = CommandType.Text;
                string fuckturnr = sqlcmd.ExecuteScalar().ToString();

                sqlcmd.CommandText = "update numbers set used = 1 where number = '" + fuckturnr + "'";
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.ExecuteNonQuery();

                sqlcmd.CommandText = "select max(gbkmut.ID) from gbkmut";
                sqlcmd.CommandType = CommandType.Text;
                string maxId = sqlcmd.ExecuteScalar().ToString();

                DateTime dated = Convert.ToDateTime(transactionDate);
                string bkjrcode = dated.ToString("yyyy");
                string periode = Convert.ToInt16(dated.ToString("MM")).ToString().PadLeft(3);
                string datum = dated.ToString("yyyy-MM-dd");

                sqlcmd.CommandText = "SELECT case when max(StringValue) is null then '000001' else max(StringValue) end as StringValue from settings " +
                            "where settinggroup = 'bjdata_;" + bkjrcode.Substring(3, 1) + "' AND settingname = '" + dagbknr + "bkstnr'";
                sqlcmd.CommandType = CommandType.Text;
                string stringVal = sqlcmd.ExecuteScalar().ToString();

                string bjdata = bkjrcode.Substring(3, 1);

                if (bjdata == "0")
                {
                    bjdata = "9";
                }
                else
                {
                    bjdata = (Convert.ToInt16(bjdata) - 1).ToString();
                }

                if (stringVal == "000001")
                {
                    string lastStringVal = "";

                    sqlcmd.CommandText = "SELECT case when max(StringValue) is null then '000001' else max(StringValue) end as StringValue " +
                                "from settings where settinggroup = 'bjdata_;" + bjdata + "' AND settingname = '" + dagbknr + "bkstnr'";
                    sqlcmd.CommandType = CommandType.Text;
                    lastStringVal = sqlcmd.ExecuteScalar().ToString();

                    string _newStringVal = (Convert.ToInt64(lastStringVal.Substring(1, 3)) + 10).ToString() + "00001";

                    sqlcmd.CommandText = "insert into settings(valuetype,longvalue,stringvalue,settinggroup,settingname)" +
                                "values(1,NULL,@stringValue,'bjdata_;" + bkjrcode.Substring(3, 1) + "','" + dagbknr + "bkstnr')";
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@stringValue", _newStringVal);
                    sqlcmd.ExecuteNonQuery();

                    stringVal = _newStringVal.ToString().PadLeft(8, '0');
                }

                string CurStringVal = stringVal.ToString().PadLeft(8, '0');
                string newStringVal = (Convert.ToInt32(stringVal) + 1).ToString().PadLeft(8, '0');

                sqlcmd.CommandText = "UPDATE settings set StringValue = '" + newStringVal + "' FROM settings WHERE SettingGroup = 'bjdata_;" + bkjrcode.Substring(3, 1) + "' AND SettingName = '" + dagbknr + "bkstnr';";
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.ExecuteNonQuery();

                sqlcmd.CommandText = "select newid()";
                sqlcmd.CommandType = CommandType.Text;
                string newID = sqlcmd.ExecuteScalar().ToString();

                string lastDigit = bkjrcode.Substring(3, 1);

                sqlcmd.CommandText = "SELECT case when max(longvalue) is null then '0' else max(longvalue) end as longvalue FROM settings WHERE settinggroup = 'bjdata_;" + lastDigit + "' AND settingname = 'verwnr';";
                sqlcmd.CommandType = CommandType.Text;
                string longValueS1 = sqlcmd.ExecuteScalar().ToString();

                if (longValueS1 == "0")
                {
                    longValueS1 = "11" + lastDigit + "0000001";

                    sqlcmd.CommandText = "insert into settings(valuetype,longvalue,stringvalue,settinggroup,settingname)" +
                        "values(1,@longValue,NULL,'bjdata_;" + bkjrcode.Substring(3, 1) + "','verwnr')";
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@longValue", longValueS1);
                    sqlcmd.ExecuteNonQuery();
                }

                string yr = dated.ToString("yyyy");
                string month = Convert.ToInt16(dated.ToString("MM")).ToString().PadLeft(3);
                string reg1 = "1".PadLeft(4);
                string reg2 = "2".PadLeft(4);

                sqlcmd.CommandText = "SELECT case when max(volgnr5) is null then '    0' else max(volgnr5) end as volgnr5 FROM amutak WHERE dagbknr = " + dagbknr + " AND bkjrcode = " + yr + " AND periode = '" + month + "'";
                sqlcmd.CommandType = CommandType.Text;
                string volgnr3 = sqlcmd.ExecuteScalar().ToString();
                string volgnr4 = (Convert.ToInt32(volgnr3) + 1).ToString();
                string volgnr5 = volgnr4.PadLeft(5);

                string newBkstnr = CurStringVal;

                sqlcmd.CommandText = "select case when max(eindsaldo) is null then '1' else max(eindsaldo) end as eindsaldo from amutak where dagbknr = " + dagbknr + " and bkstnr = '" + stringVal + "'";
                sqlcmd.CommandType = CommandType.Text;
                string beginsaldo = sqlcmd.ExecuteScalar().ToString();

                sqlcmd.CommandText = "select reknr from dagbk where dagbknr = '" + dagbknr + "'";
                sqlcmd.CommandType = CommandType.Text;
                string reknr3 = sqlcmd.ExecuteScalar().ToString();

                double endsaldo;

                if (debitGL == "1000006")
                {
                    endsaldo = Convert.ToDouble(beginsaldo) - amount;
                }
                else
                {
                    endsaldo = Convert.ToDouble(beginsaldo) + amount;
                }

                #region INSERT AMUTAK
                try
                {
                    sqlcmd.CommandText = "Insert_Amutak";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt16(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr);
                    sqlcmd.Parameters.AddWithValue("@volgnr5", volgnr5);
                    sqlcmd.Parameters.AddWithValue("@beginsaldo", beginsaldo);
                    sqlcmd.Parameters.AddWithValue("@eindsaldo", endsaldo);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", newBkstnr);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    //sqlcmd.Parameters.AddWithValue("@crdnr", crdnr);
                    sqlcmd.Parameters.AddWithValue("@crdnr", "000000");
                    sqlcmd.Parameters.AddWithValue("@bedrag", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@valcode", currency);
                    sqlcmd.Parameters.AddWithValue("@koers", qty1);
                    sqlcmd.Parameters.AddWithValue("@val_bdr", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@betaalref", stringNull);
                    sqlcmd.Parameters.AddWithValue("@betcond", stringNull);

                    sqlcmd.Parameters.AddWithValue("@kredbep", "K");
                    sqlcmd.Parameters.AddWithValue("@bdrkredbep", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@vervdtkrd2", datum);
                    sqlcmd.Parameters.AddWithValue("@percentag2", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@grek_bdr", doubleZero);

                    sqlcmd.Parameters.AddWithValue("@reknr", stringNull);
                    sqlcmd.Parameters.AddWithValue("@banksubtyp", stringNull);
                    sqlcmd.Parameters.AddWithValue("@docnumber", stringNull);
                    sqlcmd.Parameters.AddWithValue("@docdate", datum);
                    sqlcmd.Parameters.AddWithValue("@entryorigin", stringN);
                    sqlcmd.Parameters.AddWithValue("@del_res_identry", intZero);
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@match_nr", stringNull);
                    sqlcmd.Parameters.AddWithValue("@syscreator", empcode);
                    sqlcmd.Parameters.AddWithValue("@amktext", intZero);
                    sqlcmd.Parameters.AddWithValue("@sysmodified", dated);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf1", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf2", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@sysmodifier", empcode);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf3", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf5", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@wisselkrs", Convert.ToDouble("1"));

                    Guid a = new Guid(newID);

                    sqlcmd.Parameters.Add("@cmp_wwn", SqlDbType.UniqueIdentifier).Value = a;
                    sqlcmd.Parameters.Add("@sysguid", SqlDbType.UniqueIdentifier).Value = a;
                    sqlcmd.Parameters.Add("@guids", SqlDbType.UniqueIdentifier).Value = a;

                    sqlcmd.Parameters.AddWithValue("@blockoutstandingItem", intZero);
                    sqlcmd.Parameters.AddWithValue("@bankacc", stringNull);
                    sqlcmd.Parameters.AddWithValue("@entrytype", stringN);
                    sqlcmd.Parameters.AddWithValue("@freefield4", stringZero);
                    sqlcmd.Parameters.AddWithValue("@freefield5", stringZero);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING AMUTAK ERROR : " + ex.ToString());
                    return false;
                }
                #endregion

                double negativeAmount = amount * -1;
                string bbcode = "0" + bcode + "-" + bcode;

                sqlcmd.CommandText = "select newid()";
                sqlcmd.CommandType = CommandType.Text;
                string newIDs = sqlcmd.ExecuteScalar().ToString();

                #region INSERT AMUTAS (Positive)
                try
                {
                    sqlcmd.CommandText = "Insert_amutas";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt16(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr);
                    sqlcmd.Parameters.AddWithValue("@volgnr5", volgnr5);
                    sqlcmd.Parameters.AddWithValue("@regel", reg1);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", newBkstnr);
                    sqlcmd.Parameters.AddWithValue("@reknr", debitGL);
                    sqlcmd.Parameters.AddWithValue("@oms25", description);
                    //sqlcmd.Parameters.AddWithValue("@crdnr", crdnr);
                    sqlcmd.Parameters.AddWithValue("@crdnr", "000000");
                    sqlcmd.Parameters.AddWithValue("@faktuurnr", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@btw_code", "0.0");
                    sqlcmd.Parameters.AddWithValue("@bedrag", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@btw_bdr", amount);
                    sqlcmd.Parameters.AddWithValue("@btw_ground", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@valcode", currency);
                    sqlcmd.Parameters.AddWithValue("@koers", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@val_bdr", amount);
                    sqlcmd.Parameters.AddWithValue("@valbtw_bdr", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@verschil", stringZero);
                    sqlcmd.Parameters.AddWithValue("@kstplcode", bbcode);
                    sqlcmd.Parameters.AddWithValue("@aantal", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@betwijze", stringN);
                    sqlcmd.Parameters.AddWithValue("@kredbep", stringN);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbep", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbp2", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@storno", intZero);
                    sqlcmd.Parameters.AddWithValue("@exvalcode", stringN);
                    sqlcmd.Parameters.AddWithValue("@wisselkrs", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@koers3", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@exvalbdr", amount);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf1", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf2", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf3", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf4", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf5", doubleZero);
                    sqlcmd.Parameters.AddWithValue("@levverw", stringN);
                    sqlcmd.Parameters.AddWithValue("@betcond", stringN);
                    sqlcmd.Parameters.AddWithValue("@vooruitbet", intZero);
                    sqlcmd.Parameters.AddWithValue("@afldat", datum);
                    sqlcmd.Parameters.AddWithValue("@voucher", intZero);
                    sqlcmd.Parameters.AddWithValue("@docnumber", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@docdate", datum);
                    sqlcmd.Parameters.AddWithValue("@entryorigin", stringN);
                    sqlcmd.Parameters.AddWithValue("@res_id", empcode);
                    sqlcmd.Parameters.AddWithValue("@crdnote", intZero);
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@syscreator", empcode);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@StatisticalFactor", 0.0);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING AMUTAS(Positive) ERROR : " + ex.ToString());
                    return false;
                }
                #endregion

                #region INSERT AMUTAS (Negative)
                try
                {
                    sqlcmd.CommandText = "Insert_amutas";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt16(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr);
                    sqlcmd.Parameters.AddWithValue("@volgnr5", volgnr5);
                    sqlcmd.Parameters.AddWithValue("@regel", reg2);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", newBkstnr);
                    sqlcmd.Parameters.AddWithValue("@reknr", creditGL);
                    sqlcmd.Parameters.AddWithValue("@oms25", description);
                    //sqlcmd.Parameters.AddWithValue("@crdnr", crdnr);
                    sqlcmd.Parameters.AddWithValue("@crdnr", "000000");
                    sqlcmd.Parameters.AddWithValue("@faktuurnr", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@btw_code", "0.0");
                    sqlcmd.Parameters.AddWithValue("@bedrag", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@btw_bdr", Convert.ToDouble(negativeAmount));
                    sqlcmd.Parameters.AddWithValue("@btw_ground", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@valcode", currency);
                    sqlcmd.Parameters.AddWithValue("@koers", Convert.ToDouble(quantityCredit));
                    sqlcmd.Parameters.AddWithValue("@val_bdr", Convert.ToDouble(negativeAmount));
                    sqlcmd.Parameters.AddWithValue("@valbtw_bdr", 0.0);
                    sqlcmd.Parameters.AddWithValue("@verschil", "0");
                    sqlcmd.Parameters.AddWithValue("@kstplcode", bbcode);
                    sqlcmd.Parameters.AddWithValue("@aantal", Convert.ToDouble(quantityCredit));
                    sqlcmd.Parameters.AddWithValue("@betwijze", "N");
                    sqlcmd.Parameters.AddWithValue("@kredbep", "N");
                    sqlcmd.Parameters.AddWithValue("@bdrkredbep", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbp2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@storno", 0);
                    sqlcmd.Parameters.AddWithValue("@exvalcode", "N");
                    sqlcmd.Parameters.AddWithValue("@wisselkrs", 0.0);
                    sqlcmd.Parameters.AddWithValue("@koers3", Convert.ToDouble(quantityCredit));
                    sqlcmd.Parameters.AddWithValue("@exvalbdr", Convert.ToDouble(negativeAmount));
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf1", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bedr_vvaf5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@levverw", "N");
                    sqlcmd.Parameters.AddWithValue("@betcond", "N");
                    sqlcmd.Parameters.AddWithValue("@vooruitbet", 0);
                    sqlcmd.Parameters.AddWithValue("@afldat", datum);
                    sqlcmd.Parameters.AddWithValue("@voucher", 0);
                    sqlcmd.Parameters.AddWithValue("@docnumber", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@docdate", datum);
                    sqlcmd.Parameters.AddWithValue("@entryorigin", "N");
                    sqlcmd.Parameters.AddWithValue("@res_id", empcode);
                    sqlcmd.Parameters.AddWithValue("@crdnote", 0);
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@syscreator", empcode);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@StatisticalFactor", 0.0);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING AMUTAS(Negative) ERROR : " + ex.ToString());
                    return false;
                }
                #endregion

                sqlcmd.CommandText = "select reknr from dagbk where dagbknr = '" + dagbknr + "';";
                sqlcmd.CommandType = CommandType.Text;
                string reknr = sqlcmd.ExecuteScalar().ToString();

                string yrOld = DateTime.Now.ToString("yyyy");
                string yrNew = yrOld + "000000";

                sqlcmd.CommandText = "SELECT case when max(longvalue) is null then " + yrNew + " else max(longvalue) end as longvalue FROM settings WHERE settinggroup = 'bjdata_;" + yr.Substring(3, 1) + "' AND settingname = '" + dagbknr + "dbkvrw';";
                sqlcmd.CommandType = CommandType.Text;
                string longValue = sqlcmd.ExecuteScalar().ToString();

                longValue = (Convert.ToInt32(longValue) + 1).ToString();

                sqlcmd.CommandText = "UPDATE settings set LongValue = '" + longValue + "' FROM settings WHERE SettingGroup = 'bjdata_;" + yr.Substring(3, 1) + "' AND SettingName = '" + dagbknr + "dbkvrw';";
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.ExecuteNonQuery();

                string newLongValueAdd = (Convert.ToInt32(longValueS1) + 1).ToString();

                sqlcmd.CommandText = "UPDATE settings set LongValue = '" + newLongValueAdd + "' FROM settings WHERE SettingGroup = 'bjdata_;" + lastDigit + "' AND SettingName = 'verwnr';";
                sqlcmd.CommandType = CommandType.Text;
                sqlcmd.ExecuteNonQuery();

                #region INSERT GBKMUT (Positive)
                try
                {
                    sqlcmd.CommandText = "Insert_Gbkmut";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt16(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@reknr", debitGL);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", newBkstnr);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr);
                    sqlcmd.Parameters.AddWithValue("@oms25", description);
                    sqlcmd.Parameters.AddWithValue("@bdr_hfl", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@btw_code", "0");
                    sqlcmd.Parameters.AddWithValue("@btw_bdr_3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@btw_grond", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@tegreknr", debitGL);
                    sqlcmd.Parameters.AddWithValue("@crdnr", "000000");
                    sqlcmd.Parameters.AddWithValue("@kstplcode", bbcode);
                    sqlcmd.Parameters.AddWithValue("@aantal", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@valcode", currency);
                    sqlcmd.Parameters.AddWithValue("@exvalcode", currency);
                    sqlcmd.Parameters.AddWithValue("@koers", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@wisselkrs", 0.0);
                    sqlcmd.Parameters.AddWithValue("@koers3", Convert.ToDouble(quantityDebit));
                    sqlcmd.Parameters.AddWithValue("@bdr_val", amount);
                    sqlcmd.Parameters.AddWithValue("@dbk_verwnr", Convert.ToInt32(longValue));
                    sqlcmd.Parameters.AddWithValue("@verwerknrl", Convert.ToInt32(longValueS1));
                    sqlcmd.Parameters.AddWithValue("@volgnr5", volgnr5);
                    sqlcmd.Parameters.AddWithValue("@regel", reg1);
                    sqlcmd.Parameters.AddWithValue("@regelcode", "B");
                    sqlcmd.Parameters.AddWithValue("@storno", 0);
                    sqlcmd.Parameters.AddWithValue("@betcond", "N");
                    sqlcmd.Parameters.AddWithValue("@btwper", 0.0);
                    sqlcmd.Parameters.AddWithValue("@oorsprong", "A");
                    sqlcmd.Parameters.AddWithValue("@afldat", datum);
                    sqlcmd.Parameters.AddWithValue("@docnumber", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@docdate", datum);
                    sqlcmd.Parameters.AddWithValue("@exvalbdr", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@entryorigin", "N");
                    sqlcmd.Parameters.AddWithValue("@vervdatkrd", datum);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbep", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbp2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@betaalref", "NULL");
                    sqlcmd.Parameters.AddWithValue("@btw_nummer", "NULL");
                    sqlcmd.Parameters.AddWithValue("@faktuurnr", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@bud_vers", "NULL");
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    Guid R = new Guid(newID);
                    sqlcmd.Parameters.AddWithValue("@syscreator", empcode);
                    sqlcmd.Parameters.AddWithValue("@ReminderCount", 0);
                    sqlcmd.Parameters.AddWithValue("@sysmodified", dated);
                    sqlcmd.Parameters.AddWithValue("@ReminderLayout", 0);
                    sqlcmd.Parameters.AddWithValue("@sysmodifier", empcode);
                    sqlcmd.Parameters.Add("@sysguid", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.AddWithValue("@BlockItem", 0);
                    sqlcmd.Parameters.AddWithValue("@CompanyCode", bcode);
                    sqlcmd.Parameters.AddWithValue("@res_id", empcode);
                    sqlcmd.Parameters.AddWithValue("@TransactionType", Convert.ToInt32(dagbknr));
                    sqlcmd.Parameters.AddWithValue("@CurrencyCode", currency);
                    sqlcmd.Parameters.AddWithValue("@Rate", 1.0);
                    sqlcmd.Parameters.AddWithValue("@AmountCentral", currency == "PHP" ? amount : amount * 60);
                    sqlcmd.Parameters.AddWithValue("@VatBaseAmountCentral", 0.0);
                    sqlcmd.Parameters.AddWithValue("@PaymentMethod", "K");
                    sqlcmd.Parameters.AddWithValue("@VatAmountCentral", 1.0);
                    sqlcmd.Parameters.AddWithValue("@transtype", "N");
                    sqlcmd.Parameters.AddWithValue("@transsubtype", "N");
                    sqlcmd.Parameters.AddWithValue("@freefield4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@freefield5", 0.0);
                    sqlcmd.Parameters.Add("@cmp_wwn", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.AddWithValue("@warehouse", "1");
                    sqlcmd.Parameters.Add("@orderdebtor", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.Add("@EntryGuid", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.AddWithValue("@Checked", 0);
                    sqlcmd.Parameters.AddWithValue("@Reviewed", 0);
                    sqlcmd.Parameters.Add("@BankTransactionGUID", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.AddWithValue("@CashRegisterAccount", dagbknr + "-" + bcode);
                    sqlcmd.Parameters.AddWithValue("@Original_Quantity", 0.0);
                    sqlcmd.Parameters.AddWithValue("@Discount", 0.1);
                    sqlcmd.Parameters.AddWithValue("@Shipment", "NULL");
                    sqlcmd.Parameters.AddWithValue("@IntStatUnit", 0.0);
                    sqlcmd.Parameters.AddWithValue("@IntWeight", 0.0);
                    sqlcmd.Parameters.AddWithValue("@IntComplete", 0);
                    sqlcmd.Parameters.Add("@LinkedLine", SqlDbType.UniqueIdentifier).Value = R;
                    sqlcmd.Parameters.AddWithValue("@TaxCode5", "N");
                    sqlcmd.Parameters.AddWithValue("@TaxBasis2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@StatisticalFactor", 0.0);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING GBKMUT(Positive) ERROR : " + ex.ToString());
                    return false;
                }
                #endregion

                Guid b = new Guid(newIDs);

                #region INSERT GBKMUT (Negative)
                try
                {
                    sqlcmd.CommandText = "Insert_Gbkmut";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt16(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@reknr", creditGL);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", newBkstnr);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr);
                    sqlcmd.Parameters.AddWithValue("@oms25", description);
                    sqlcmd.Parameters.AddWithValue("@bdr_hfl", currency == "PHP" ? negativeAmount : negativeAmount * 60);
                    sqlcmd.Parameters.AddWithValue("@btw_code", "0");
                    sqlcmd.Parameters.AddWithValue("@btw_bdr_3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@btw_grond", currency == "PHP" ? negativeAmount : negativeAmount * 60);
                    sqlcmd.Parameters.AddWithValue("@tegreknr", creditGL);
                    sqlcmd.Parameters.AddWithValue("@crdnr", "000000");
                    sqlcmd.Parameters.AddWithValue("@kstplcode", bbcode);
                    sqlcmd.Parameters.AddWithValue("@aantal", quantityCredit);
                    sqlcmd.Parameters.AddWithValue("@valcode", currency);
                    sqlcmd.Parameters.AddWithValue("@exvalcode", currency);
                    sqlcmd.Parameters.AddWithValue("@koers", quantityCredit);
                    sqlcmd.Parameters.AddWithValue("@wisselkrs", 0.0);
                    sqlcmd.Parameters.AddWithValue("@koers3", quantityCredit);
                    sqlcmd.Parameters.AddWithValue("@bdr_val", negativeAmount);
                    sqlcmd.Parameters.AddWithValue("@dbk_verwnr", longValue);
                    sqlcmd.Parameters.AddWithValue("@verwerknrl", longValueS1);
                    sqlcmd.Parameters.AddWithValue("@volgnr5", volgnr5);
                    sqlcmd.Parameters.AddWithValue("@regel", reg2);
                    sqlcmd.Parameters.AddWithValue("@regelcode", "B");
                    sqlcmd.Parameters.AddWithValue("@storno", Convert.ToInt16("0"));
                    sqlcmd.Parameters.AddWithValue("@betcond", "N");
                    sqlcmd.Parameters.AddWithValue("@btwper", 0.0);
                    sqlcmd.Parameters.AddWithValue("@oorsprong", "A");
                    sqlcmd.Parameters.AddWithValue("@afldat", datum);
                    sqlcmd.Parameters.AddWithValue("@docnumber", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@docdate", datum);
                    sqlcmd.Parameters.AddWithValue("@exvalbdr", currency == "PHP" ? negativeAmount : negativeAmount * 60);
                    sqlcmd.Parameters.AddWithValue("@entryorigin", "N");
                    sqlcmd.Parameters.AddWithValue("@vervdatkrd", datum);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbep", 0.0);
                    sqlcmd.Parameters.AddWithValue("@bdrkredbp2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@betaalref", "NULL");
                    sqlcmd.Parameters.AddWithValue("@btw_nummer", "NULL");
                    sqlcmd.Parameters.AddWithValue("@faktuurnr", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@bud_vers", "NULL");
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@syscreator", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@ReminderCount", Convert.ToInt32(0));
                    sqlcmd.Parameters.AddWithValue("@sysmodified", dated);
                    sqlcmd.Parameters.AddWithValue("@ReminderLayout", Convert.ToInt32(0));
                    sqlcmd.Parameters.AddWithValue("@sysmodifier", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.Add("@sysguid", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.AddWithValue("@BlockItem", Convert.ToInt16(0));
                    sqlcmd.Parameters.AddWithValue("@CompanyCode", bcode);
                    sqlcmd.Parameters.AddWithValue("@res_id", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@TransactionType", Convert.ToInt32(dagbknr));
                    sqlcmd.Parameters.AddWithValue("@CurrencyCode", currency);
                    sqlcmd.Parameters.AddWithValue("@Rate", 1.0);
                    sqlcmd.Parameters.AddWithValue("@AmountCentral", currency == "PHP" ? negativeAmount : negativeAmount * 60);
                    sqlcmd.Parameters.AddWithValue("@VatBaseAmountCentral", 0.0);
                    sqlcmd.Parameters.AddWithValue("@PaymentMethod", "K");
                    sqlcmd.Parameters.AddWithValue("@VatAmountCentral", 1.0);
                    sqlcmd.Parameters.AddWithValue("@transtype", "N");
                    sqlcmd.Parameters.AddWithValue("@transsubtype", "N");
                    sqlcmd.Parameters.AddWithValue("@freefield4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@freefield5", 0.0);
                    sqlcmd.Parameters.Add("@cmp_wwn", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.AddWithValue("@warehouse", "1");
                    sqlcmd.Parameters.Add("@orderdebtor", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.Add("@EntryGuid", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.Add("@TransactionGuid", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.AddWithValue("@Checked", 0);
                    sqlcmd.Parameters.AddWithValue("@Reviewed", 0);
                    sqlcmd.Parameters.Add("@BankTransactionGUID", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.AddWithValue("@CashRegisterAccount", dagbknr + "-" + bcode);
                    sqlcmd.Parameters.AddWithValue("@Original_Quantity", 0.0);
                    sqlcmd.Parameters.AddWithValue("@Discount", 0.0);
                    sqlcmd.Parameters.AddWithValue("@Shipment", "NULL");
                    sqlcmd.Parameters.AddWithValue("@IntStatUnit", 0.0);
                    sqlcmd.Parameters.AddWithValue("@IntWeight", 0.0);
                    sqlcmd.Parameters.AddWithValue("@IntComplete", 0);
                    sqlcmd.Parameters.Add("@LinkedLine", SqlDbType.UniqueIdentifier).Value = b;
                    sqlcmd.Parameters.AddWithValue("@TaxCode5", "N");
                    sqlcmd.Parameters.AddWithValue("@TaxBasis2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxBasis5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount2", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount3", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount4", 0.0);
                    sqlcmd.Parameters.AddWithValue("@TaxAmount5", 0.0);
                    sqlcmd.Parameters.AddWithValue("@StatisticalFactor", 0.0);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING GBKMUT(Negative) ERROR : " + ex.ToString());
                    return false;
                }
                #endregion

                string dagbknr1 = "";
                string bkstnr1 = "";
                string bdr_hfl1 = "";

                sqlcmd.CommandText = "select dagbknr,bkstnr,sum(bdr_hfl) as result from gbkmut where gbkmut.ID > " + maxId + " group by dagbknr,bkstnr";
                sqlcmd.CommandType = CommandType.Text;

                using (SqlDataReader dr = sqlcmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dagbknr1 = dr["dagbknr"].ToString();
                            bkstnr1 = dr["bkstnr"].ToString();
                            bdr_hfl1 = dr["result"].ToString();
                        }
                    }
                }

                try
                {
                    sqlcmd.CommandText = "Insert_Balance";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@CompanyCode", bcode);
                    sqlcmd.Parameters.AddWithValue("@FinYear", Convert.ToInt32(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@FinPeriod", periode);
                    sqlcmd.Parameters.AddWithValue("@CompanyAccountCode", creditGL);
                    sqlcmd.Parameters.AddWithValue("@CompanyCostcenterCode", bbcode);
                    sqlcmd.Parameters.AddWithValue("@CompanyCostunitCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@CurrencyCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@AmountDebit", 0.0);
                    sqlcmd.Parameters.AddWithValue("@AmountCredit", amount);
                    sqlcmd.Parameters.AddWithValue("@CurrencyAliasAC", currency);
                    sqlcmd.Parameters.AddWithValue("@AmountDebitAC", 0.0);
                    sqlcmd.Parameters.AddWithValue("@AmountCreditAC", amount);
                    sqlcmd.Parameters.AddWithValue("@transtype", "N");
                    sqlcmd.Parameters.AddWithValue("@Warehouse", "1");
                    sqlcmd.Parameters.AddWithValue("@ItemCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@Quantity", Convert.ToDouble(qty1));

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING BALANCE ERROR : " + ex.ToString());
                    return false;
                }

                try
                {
                    sqlcmd.CommandText = "Insert_verslg";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@verwerknr", Convert.ToInt32(longValueS1));
                    sqlcmd.Parameters.AddWithValue("@bkjrcode", Convert.ToInt32(bkjrcode));
                    sqlcmd.Parameters.AddWithValue("@periode", periode);
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr1);
                    sqlcmd.Parameters.AddWithValue("@oms40", "Financial posting" + " " + empFullname);
                    sqlcmd.Parameters.AddWithValue("@datum", datum);
                    sqlcmd.Parameters.AddWithValue("@datum_verd", datum);
                    sqlcmd.Parameters.AddWithValue("@controle", 0);
                    sqlcmd.Parameters.AddWithValue("@tot_debet", amount);
                    sqlcmd.Parameters.AddWithValue("@tot_credit", amount);
                    sqlcmd.Parameters.AddWithValue("@mutafsl", 0);
                    sqlcmd.Parameters.AddWithValue("@notities", 0);
                    sqlcmd.Parameters.AddWithValue("@user_id", empcode);
                    sqlcmd.Parameters.AddWithValue("@aant_afgdr", 0);
                    Guid rm = new Guid(newID);
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@syscreator", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@sysmodified", dated);
                    sqlcmd.Parameters.AddWithValue("@sysmodifier", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.Add("@sysguid", SqlDbType.UniqueIdentifier).Value = rm;

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING VERSLG ERROR : " + ex.ToString());
                    return false;
                }

                try
                {
                    sqlcmd.CommandText = "Insert_BankTransactions";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@Type", "W");
                    sqlcmd.Parameters.AddWithValue("@OwnBankAccount", dagbknr1 + "-" + bcode);
                    sqlcmd.Parameters.AddWithValue("@BatchNumber", 0);
                    sqlcmd.Parameters.AddWithValue("@TransactionType", "Z");
                    sqlcmd.Parameters.AddWithValue("@TransactionNumber", "NULL");
                    sqlcmd.Parameters.AddWithValue("@Status", "J");
                    sqlcmd.Parameters.AddWithValue("@PaymentMethod", "T");
                    sqlcmd.Parameters.AddWithValue("@CreditorNumber", "000000");
                    sqlcmd.Parameters.AddWithValue("@DebtorNumber", "NULL");
                    sqlcmd.Parameters.AddWithValue("@ExchangeRate", 1.0);
                    sqlcmd.Parameters.AddWithValue("@TCCode", currency);
                    sqlcmd.Parameters.AddWithValue("@AmountDC", amount);
                    sqlcmd.Parameters.AddWithValue("@AmountTC", amount);
                    sqlcmd.Parameters.AddWithValue("@OffsetBankCountry", "N");
                    //sqlcmd.Parameters.AddWithValue("@OffsetName", AM);
                    sqlcmd.Parameters.AddWithValue("@OffsetName", "Cash");
                    sqlcmd.Parameters.AddWithValue("@OffsetAddressLine1", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetAddressLine2", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetAddressLine3", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetBankAccount", "NULLL");
                    sqlcmd.Parameters.AddWithValue("@OffsetPostalCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetCity", "UNKNOWN");
                    sqlcmd.Parameters.AddWithValue("@OffsetReference", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@OffsetCountryCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetBankName", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetBankSWIFTCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OwnReference", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@Description", description);
                    sqlcmd.Parameters.AddWithValue("@Blocked", 0);
                    sqlcmd.Parameters.AddWithValue("@ProcessingDate", datum);
                    sqlcmd.Parameters.AddWithValue("@InvoiceNumber", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@StatementType", "K");
                    sqlcmd.Parameters.AddWithValue("@StatementDate", datum);
                    sqlcmd.Parameters.AddWithValue("@StatementNumber", "0");
                    sqlcmd.Parameters.AddWithValue("@StatementLineNumber", "1");
                    sqlcmd.Parameters.AddWithValue("@ValueDate", datum);
                    sqlcmd.Parameters.AddWithValue("@FileName", "NULL");
                    sqlcmd.Parameters.AddWithValue("@LedgerAccount", "NULL");
                    sqlcmd.Parameters.AddWithValue("@OffsetLedgerAccountNumber", "NULL");
                    sqlcmd.Parameters.AddWithValue("@EntryNumber", bkstnr1);
                    sqlcmd.Parameters.AddWithValue("@SupplierInvoiceNumber", "NULL");
                    sqlcmd.Parameters.AddWithValue("@DueDate", dated);
                    sqlcmd.Parameters.AddWithValue("@HumanResourceID", 0);
                    sqlcmd.Parameters.AddWithValue("@MatchID", 0);
                    sqlcmd.Parameters.AddWithValue("@OffsetIdentificationNumberBank", "NULL");
                    sqlcmd.Parameters.AddWithValue("@PaymentType", "K");
                    sqlcmd.Parameters.AddWithValue("@bedrnr", bcode);
                    sqlcmd.Parameters.AddWithValue("@InvoiceDate", datum);
                    sqlcmd.Parameters.AddWithValue("@Prepayment", 0);
                    sqlcmd.Parameters.AddWithValue("@PaymentCondition", "00");
                    sqlcmd.Parameters.AddWithValue("@OrderNumber", "N");
                    sqlcmd.Parameters.AddWithValue("@InvoiceCode", "N");
                    Guid R2 = new Guid(newID);
                    sqlcmd.Parameters.AddWithValue("@SequenceNumber", "NULL");
                    sqlcmd.Parameters.Add("@DocAttachmentID", SqlDbType.UniqueIdentifier).Value = R2;
                    sqlcmd.Parameters.AddWithValue("@Approver", 0);
                    sqlcmd.Parameters.AddWithValue("@Approved", dated);
                    sqlcmd.Parameters.AddWithValue("@VATCode", "0");
                    sqlcmd.Parameters.AddWithValue("@Processor", 0);
                    sqlcmd.Parameters.AddWithValue("@Processed", dated);
                    sqlcmd.Parameters.AddWithValue("@Approver2", 0);
                    sqlcmd.Parameters.AddWithValue("@Approved2", dated);
                    sqlcmd.Parameters.AddWithValue("@Journalizer", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@Journalized", dated);
                    sqlcmd.Parameters.AddWithValue("@TermPercentage", 0.0);
                    sqlcmd.Parameters.AddWithValue("@DepositDate", dated);
                    sqlcmd.Parameters.AddWithValue("@DepositNumber", 0);
                    sqlcmd.Parameters.AddWithValue("@PaymentDays", 0);
                    sqlcmd.Parameters.Add("@DocumentID", SqlDbType.UniqueIdentifier).Value = R2;
                    sqlcmd.Parameters.AddWithValue("@Warehouse", "1");
                    sqlcmd.Parameters.AddWithValue("@ExtraCurrencyCode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@ExtraCurrencyAmount", 0.0);
                    sqlcmd.Parameters.AddWithValue("@InstrumentStatus", "N");
                    sqlcmd.Parameters.AddWithValue("@InstrumentReference", 0);
                    sqlcmd.Parameters.AddWithValue("@InstrumentBank", "NULL");
                    sqlcmd.Parameters.AddWithValue("@MaturityDays", 0);
                    sqlcmd.Parameters.AddWithValue("@OwnBankAccountRef", dagbknr1 + "-" + bcode);
                    sqlcmd.Parameters.AddWithValue("@AdvanceInvoiceNumber", "NULL");
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);
                    sqlcmd.Parameters.AddWithValue("@syscreator", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@sysmodified", dated);
                    sqlcmd.Parameters.AddWithValue("@sysmodifier", Convert.ToInt32(empcode));

                    sqlcmd.Parameters.Add("@sysguid", SqlDbType.UniqueIdentifier).Value = R2;

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING BANK TRANSACTION ERROR : " + ex.ToString());
                    return false;
                }

                try
                {
                    sqlcmd.CommandText = "SP_Insert_tbltrn_bos_entry";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@statementnumber", bkstnr1);
                    sqlcmd.Parameters.AddWithValue("@resourceid", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@journalcode", dagbknr1);
                    sqlcmd.Parameters.AddWithValue("@GLAccount", creditGL);
                    sqlcmd.Parameters.AddWithValue("@gldescription", description);
                    sqlcmd.Parameters.AddWithValue("@amount", amount);
                    sqlcmd.Parameters.AddWithValue("@accountcode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@customercode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@customername", "NULL");
                    sqlcmd.Parameters.AddWithValue("@yourreference", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@ourreference", fuckturnr);
                    sqlcmd.Parameters.AddWithValue("@itemcode", "NULL");
                    sqlcmd.Parameters.AddWithValue("@description", description);
                    sqlcmd.Parameters.AddWithValue("@CostCenter", bbcode);
                    sqlcmd.Parameters.AddWithValue("@financenumber", Convert.ToInt32(longValueS1));
                    sqlcmd.Parameters.AddWithValue("@transactiontype", "NULL");
                    sqlcmd.Parameters.AddWithValue("@transactiondate", dated);
                    sqlcmd.Parameters.AddWithValue("@currency", currency);
                    sqlcmd.Parameters.AddWithValue("@xrate", 0.0);
                    sqlcmd.Parameters.AddWithValue("@syscreator", Convert.ToInt32(empcode));
                    sqlcmd.Parameters.AddWithValue("@syscreated", dated);

                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING BOS ENTRY ERROR : " + ex.ToString());
                    return false;
                }

                try
                {
                    sqlcmd.CommandText = "insert into tbl_XMLUploadTrans (journalcode,entrynum,transid,datecreated,Uploaded)" +
                               "values (@dagbknr,@bkstnr,newID(),@datecreated,@Uploaded)";
                    sqlcmd.CommandType = CommandType.Text;
                    sqlcmd.Parameters.Clear();
                    sqlcmd.Parameters.AddWithValue("@dagbknr", dagbknr1);
                    sqlcmd.Parameters.AddWithValue("@bkstnr", bkstnr1);
                    sqlcmd.Parameters.AddWithValue("@datecreated", dated);
                    sqlcmd.Parameters.AddWithValue("@Uploaded", "0");
                    sqlcmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    _logger.Error("INSERTING XML UPLOAD TRANS ERROR : " + ex.ToString());
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return false;
            }
        }
    }
}
